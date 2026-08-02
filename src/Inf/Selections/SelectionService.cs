using Core.Selections.Models;
using Core.Settings.Services;
using Inf.Constants;
using Inf.FileSystem;
using System.Text.Json;
using ISelectionService = Core.Selections.Services.ISelectionService;

namespace Inf.Selections
{
    public class SelectionService(IFileSystemWrapper fs, IConfigService config, IFileSystemConstants constants) : ISelectionService
    {
        public async Task<List<Selection>> GetSelections()
        {
            if (!config.ProjectOpened)
                return [];
            var filePath = fs.PathCombine(config.RootDirectory, constants.LocalFolderName, constants.SelectionsFileName);
            if (!fs.FileExist(filePath))
                return [];

            string json = await fs.FileReadAsync(filePath);
            List<Selection> selections = JsonSerializer.Deserialize<List<Selection>>(json) ?? [];
            return selections;
        }

        public async Task SaveSelections(IEnumerable<Selection> selections)
        {
            if (!config.ProjectOpened)
                return;
            var dir = fs.PathCombine(config.RootDirectory, constants.LocalFolderName);
            var filePath = fs.PathCombine(config.RootDirectory, constants.LocalFolderName, constants.SelectionsFileName);

            if (fs.FileExist(filePath))
                fs.FileDelete(filePath);

            if (!fs.DirectoryExist(dir))
                fs.CreateDirectory(dir);
            await fs.FileWriteAsync(filePath, JsonSerializer.Serialize(selections));
        }
    }
}
