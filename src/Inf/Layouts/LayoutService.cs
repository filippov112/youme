using Core.Layouts.DTO;
using Core.Layouts.Model;
using Core.Layouts.Services;
using Core.Settings.Services;
using Inf.Constants;
using Inf.FileSystem;
using System.Text.Json;

namespace Inf.Layouts
{
    public class LayoutService : ILayoutService
    {
        private readonly IFileSystemWrapper _fileSystemWrapper;
        private readonly IConfigService _configService;
        private readonly IFileSystemConstants _systemConstants;
        public LayoutService(IFileSystemWrapper fileSystemWrapper, IConfigService configService, IFileSystemConstants systemConstants)
        {
            _fileSystemWrapper = fileSystemWrapper;
            _configService = configService;
            _systemConstants = systemConstants;

            // Инициализируем файлы при первом запуске
            string activeLayout = ActiveLayoutStructure;
        }
        public string ActiveLayoutStructure
        {
            get
            {
                // Если установлена
                if (_activeLayout is not null)
                    return _activeLayout.Structure;

                // Если не установлена, читаем файл и устанавливаем
                var layouts = Task.Run(async () => await GetLayouts(true, true)).Result;
                _activeLayout = layouts.FirstOrDefault(x => x.IsActive);
                if (_activeLayout is not null)
                    return _activeLayout.Structure;

                // Если файл не пустой, то берем первую компоновку
                if (layouts.Count > 0)
                {
                    _activeLayout = layouts.First();
                    _activeLayout.IsActive = true;
                    Task.Run(async () => await SaveAllLayouts(layouts));
                    return _activeLayout.Structure;
                }

                // Если файл отсутствует, создаем его с компоновкой по умолчанию и устанавливаем
                var conf = Task.Run(async () => await _configService.GetConfigAsync()).Result;
                String queryKey = "###query###";
                String contextKey = "###context###";
                if (conf is not null)
                {
                    queryKey = conf.Local is null ? conf.Global.QueryKey : conf.Local.QueryKey;
                    contextKey = conf.Local is null ? conf.Global.ContextKey : conf.Local.ContextKey;
                }
                _activeLayout = new() { ID = Guid.NewGuid(), Name = "Default Layout", IsActive = true, Structure = $"{queryKey}\n''''\n{contextKey}\n''''" };
                Task.Run(async () => await SaveAllLayouts([new(_activeLayout, true),]));
                return _activeLayout.Structure;
            }
        }
        public Guid ActiveLayoutID => _activeLayout?.ID ?? Guid.Empty;

        private Layout? _activeLayout;
        public async Task ChangeActiveLayout(Guid id)
        {
            _activeLayout?.IsActive = false;
            var layouts = await GetLayouts(true, true);
            _activeLayout = layouts.FirstOrDefault(x => x.ID == id) ?? _activeLayout;
        }

        public async Task<List<LayoutDto>> GetLayouts(bool commonLayouts, bool projectLayouts)
        {
            List<LayoutDto> result = [];

            // Общие
            if (commonLayouts)
            {
                var globalPath = _fileSystemWrapper.PathCombine(_fileSystemWrapper.GetApplicationDirectory(), _systemConstants.LayoutsFileName);
                if (_fileSystemWrapper.FileExist(globalPath))
                {
                    string globalJson = await _fileSystemWrapper.FileReadAsync(globalPath);
                    List<Layout> globalLayouts = JsonSerializer.Deserialize<List<Layout>>(globalJson) ?? [];
                    result.AddRange(globalLayouts.Select(x => new LayoutDto(x, true)));
                }
            }
            
            // Проектные
            if (projectLayouts)
            {
                if (!_configService.ProjectOpened)
                    return result;
                var localPath = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName, _systemConstants.LayoutsFileName);
                if (!_fileSystemWrapper.FileExist(localPath))
                    return result;

                string localJson = await _fileSystemWrapper.FileReadAsync(localPath);
                List<Layout> localLayouts = JsonSerializer.Deserialize<List<Layout>>(localJson) ?? [];
                result.AddRange(localLayouts.Select(x => new LayoutDto(x, false)));
            }
            
            return result;
        }

        public async Task SaveAllLayouts(List<LayoutDto> layouts)
        {
            List<Layout> globalList = layouts.Where(x => x.IsCommon).Select(y => new Layout(y)).ToList();
            List<Layout> localList = layouts.Where(x => !x.IsCommon).Select(y => new Layout(y)).ToList();

            var dir = _fileSystemWrapper.PathCombine(_fileSystemWrapper.GetApplicationDirectory());
            var filePath = _fileSystemWrapper.PathCombine(_fileSystemWrapper.GetApplicationDirectory(), _systemConstants.LayoutsFileName);

            if (_fileSystemWrapper.FileExist(filePath))
                _fileSystemWrapper.FileDelete(filePath);

            if (!_fileSystemWrapper.DirectoryExist(dir))
                _fileSystemWrapper.CreateDirectory(dir);
            await _fileSystemWrapper.FileWriteAsync(filePath, JsonSerializer.Serialize(globalList));

            if (_configService.ProjectOpened)
            {
                var dir1 = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName);
                var filePath1 = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName, _systemConstants.LayoutsFileName);

                if (_fileSystemWrapper.FileExist(filePath1))
                    _fileSystemWrapper.FileDelete(filePath1);

                if (!_fileSystemWrapper.DirectoryExist(dir1))
                    _fileSystemWrapper.CreateDirectory(dir1);
                await _fileSystemWrapper.FileWriteAsync(filePath1, JsonSerializer.Serialize(localList));
            }

        }
    }
}
