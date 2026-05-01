using Application.Interfaces;
using Application.Models;
using Infrastructure.Interfaces;
using System.Text.Json;

namespace Infrastructure.Services
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
        public async Task<Config> LoadLocal(string projectDirectory)
        {
            var filePath = fs.PathCombine(
                    projectDirectory,
                    constants.LocalFolderName,
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

        public async Task SaveLocal(Config localConfig, string projectDirectory)
        {
            var filePath = fs.PathCombine(
                    projectDirectory,
                    constants.LocalFolderName,
                    constants.ConfigFileName
                    );
            fs.FileDelete(filePath);
            await fs.FileWriteAsync(filePath, JsonSerializer.Serialize(localConfig));
        }
        public async Task SaveGlobal(Config globalConfig)
        {
            var filePath = fs.PathCombine(
                    fs.GetApplicationDirectory(),
                    constants.ConfigFileName
                    );
            fs.FileDelete(filePath);
            await fs.FileWriteAsync(filePath, JsonSerializer.Serialize(globalConfig));
        }
    }
}
