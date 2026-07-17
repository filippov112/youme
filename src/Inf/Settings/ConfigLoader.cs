using Core.Settings.Models;
using Core.Settings.Services;
using Inf.Constants;
using Inf.FileSystem;
using System.Text.Json;

namespace Inf.Settings
{
    public class ConfigLoader(IFileSystemWrapper fs, IFileSystemConstants constants) : IConfigLoader
    {
        public async Task<Config> LoadGlobal()
        {
            var filePath = fs.PathCombine(
                    fs.GetApplicationDirectory(),
                    constants.ConfigFileName
                    );
            if (!fs.FileExist(filePath))
                return new Config();
            string json = await fs.FileReadAsync(filePath);
            var config = JsonSerializer.Deserialize<Config>(json);
            if (config == null)
                return new Config();
            return config;
        }
        public async Task<Config?> LoadLocal(string projectDirectory)
        {
            var filePath = fs.PathCombine(
                    projectDirectory,
                    constants.LocalFolderName,
                    constants.ConfigFileName
                    );
            if (!fs.FileExist(filePath))
                return null;
            string json = await fs.FileReadAsync(filePath);
            return JsonSerializer.Deserialize<Config>(json);
        }

        public async Task SaveLocal(Config localConfig, string projectDirectory)
        {
            var filePath = fs.PathCombine(
                    projectDirectory,
                    constants.LocalFolderName,
                    constants.ConfigFileName
                    );
            if (fs.FileExist(filePath))
                fs.FileDelete(filePath);
            var dir = fs.GetDirectoryName(filePath) ?? "";
            if (!fs.DirectoryExist(dir))
                fs.CreateDirectory(dir);
            await fs.FileWriteAsync(filePath, JsonSerializer.Serialize(localConfig));
        }
        public async Task SaveGlobal(Config globalConfig)
        {
            var dir = fs.GetApplicationDirectory() ?? "";
            var filePath = fs.PathCombine(dir, constants.ConfigFileName);
            if (fs.FileExist(filePath))
                fs.FileDelete(filePath);

            if (!fs.DirectoryExist(dir))
                fs.CreateDirectory(dir);
            await fs.FileWriteAsync(filePath, JsonSerializer.Serialize(globalConfig));
        }
    }
}
