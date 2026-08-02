using Core.Layouts.DTO;
using Core.Layouts.Model;
using Core.Layouts.Services;
using Core.Settings.Services;
using Inf.Constants;
using Inf.FileSystem;
using System.Text.Json;

namespace Inf.Layouts
{
    public class FileComponentService : IFileComponentService
    {
        private readonly IFileSystemWrapper _fileSystemWrapper;
        private readonly IConfigService _configService;
        private readonly IFileSystemConstants _systemConstants;
        public FileComponentService(IFileSystemWrapper fileSystemWrapper, IConfigService configService, IFileSystemConstants systemConstants)
        {
            _fileSystemWrapper = fileSystemWrapper;
            _configService = configService;
            _systemConstants = systemConstants;
        }
        public async Task<List<FileComponentDto>> GetAllComponents()
        {
            if (!_configService.ProjectOpened)
                return [];
            var filePath = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName, _systemConstants.ComponentsFileName);
            if (!_fileSystemWrapper.FileExist(filePath))
                return [];

            string json = await _fileSystemWrapper.FileReadAsync(filePath);
            List<FileComponent> components = JsonSerializer.Deserialize<List<FileComponent>>(json) ?? [];
            return components.Select(x => new FileComponentDto(x)).ToList();
        }

        public async Task<Dictionary<string, string>> GetComponentsDict()
        {
            if (!_configService.ProjectOpened)
                return [];
            var components = await GetAllComponents();
            Dictionary<string, string> result = [];
            foreach (var component in components)
                result[component.Key] = (await _fileSystemWrapper.FileReadAsync(Path.Combine(_configService.RootDirectory, component.Path))) ?? "";
            return result;
        }

        public async Task SaveAllComponents(List<FileComponentDto> files)
        {
            if (!_configService.ProjectOpened)
                return;
            var dir = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName);
            var filePath = _fileSystemWrapper.PathCombine(_configService.RootDirectory, _systemConstants.LocalFolderName, _systemConstants.ComponentsFileName);

            if (_fileSystemWrapper.FileExist(filePath))
                _fileSystemWrapper.FileDelete(filePath);

            if (!_fileSystemWrapper.DirectoryExist(dir))
                _fileSystemWrapper.CreateDirectory(dir);
            await _fileSystemWrapper.FileWriteAsync(filePath, JsonSerializer.Serialize(files.Select(x => new FileComponent(x)).ToList()));
        }
    }
}
