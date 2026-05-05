using Pdp.App.Interfaces;
using Pdp.App.Models;

namespace Pdp.App.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigLoader _loader;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private Config? _localConfig;
        private readonly AsyncLazy<Config> _lazyGlobalConfig;

        public string RootDirectory { get; private set; } = string.Empty;

        public ConfigService(IConfigLoader loader)
        {
            _loader = loader;
            _lazyGlobalConfig = new AsyncLazy<Config>(() => _loader.LoadGlobal());
        }

        public async Task<CombinationConfig> GetCombinationConfigAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var global = await _lazyGlobalConfig.GetValueAsync();
                var local = _localConfig;

                var current = local ?? global ?? new Config();
                return new CombinationConfig(current);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<AllConfigDto> GetAllConfigAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var global = await _lazyGlobalConfig.GetValueAsync();

                return new AllConfigDto()
                {
                    Global = new CombinationConfig(global ?? new Config()),
                    Local = _localConfig != null ? new CombinationConfig(_localConfig) : null
                };
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SaveAllConfigAsync(AllConfigDto allConfig)
        {
            await _lock.WaitAsync();
            try
            {
                var globalConfig = new Config(allConfig.Global);
                await _loader.SaveGlobal(globalConfig);

                // Сброс ленивой загрузки для глобальной конфигурации
                _lazyGlobalConfig.Reset(globalConfig);

                if (allConfig.Local != null)
                {
                    var localConfig = new Config(allConfig.Local);
                    await _loader.SaveLocal(localConfig, RootDirectory);
                    _localConfig = localConfig;
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SetRootDirectoryAsync(string rootDirectory)
        {
            await _lock.WaitAsync();
            try
            {
                RootDirectory = rootDirectory;
                _localConfig = await _loader.LoadLocal(RootDirectory);

            }
            finally
            {
                _lock.Release();
            }
        }

        public AllConfigDto GetAllConfigDefault()
        {
            return new AllConfigDto() { Global = new CombinationConfig(new Config()), Local = new CombinationConfig(new Config()) };
        }
    }
}
