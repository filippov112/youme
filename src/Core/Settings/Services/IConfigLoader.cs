using Core.Settings.Models;

namespace Core.Settings.Services
{
    public interface IConfigLoader
    {
        public Task<GlobalConfig> LoadGlobal();
        public Task<LocalConfig?> LoadLocal(string projectDirectory);
        public Task SaveLocal(LocalConfig config, string projectDirectory);
        public Task SaveGlobal(GlobalConfig config);
    }
}
