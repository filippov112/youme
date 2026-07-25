using Core.Settings.Models;
using Core.Settings.Other;

namespace Core.Settings.Services
{
    public interface IConfigService
    {
        /// <summary>
        /// Корневой каталог открытого проекта
        /// </summary>
        public string RootDirectory { get; }

        /// <summary>
        /// Открыт проект
        /// </summary>
        public bool ProjectOpened => !string.IsNullOrEmpty(RootDirectory);


        /// <summary>
        /// Метод обновления корневого каталога проекта (вызывается при открытии)
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <returns></returns>
        public Task SetRootDirectoryAsync(string rootDirectory);

        /// <summary>
        /// Метод сборки действующий в проекте параметров конфигурации 
        /// путем объединения конфигураций на уровне открытого проекта и глобального конфига.
        /// Используется сервисом сборки запроса.
        /// </summary>
        /// <returns></returns>
        public Task<CombinationConfig> GetCombinationConfigAsync();

        /// <summary>
        /// Метод чтения всех доступных в данный момент конфигураций (проектная - если открыт проект; глобальная).
        /// Вызывается при открытии окна настроек.
        /// Создает независимую копию данных (состояние не отслеживается).
        /// </summary>
        /// <returns></returns>
        public Task<AllConfigDto> GetAllConfigAsync();

        /// <summary>
        /// Метод сохранения конфигураций.
        /// Вызывается при вызове кнопки сохранения в окне настроек.
        /// </summary>
        /// <param name="allConfig"></param>
        /// <returns></returns>
        public Task SaveAllConfigAsync(AllConfigDto allConfig);

        /// <summary>
        /// Метод получения заводского набора настроек.
        /// Используется в окне настроек для сброса к заводским настройкам.
        /// Не меняет конфигурации сами по себе, а только создает копию.
        /// </summary>
        /// <returns></returns>
        public AllConfigDto GetAllConfigDefault();
    }

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
                    Local = _localConfig != null ? new CombinationConfig(_localConfig) : new CombinationConfig(global ?? new Config())
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
