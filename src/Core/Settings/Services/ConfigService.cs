using Core.Settings.Models;
using Core.Settings.Models.DTO;
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
        public Task OpenProjectAsync(string rootDirectory);

        /// <summary>
        /// Метод чтения всех доступных в данный момент конфигураций (проектная - если открыт проект; глобальная).
        /// Вызывается при открытии окна настроек.
        /// Создает независимую копию данных (состояние не отслеживается).
        /// </summary>
        /// <returns></returns>
        public Task<ConfigDto> GetConfigAsync();

        /// <summary>
        /// Метод сохранения конфигураций.
        /// Вызывается при вызове кнопки сохранения в окне настроек.
        /// </summary>
        /// <param name="allConfig"></param>
        /// <returns></returns>
        public Task SaveConfigAsync(ConfigDto allConfig);

        /// <summary>
        /// Метод получения заводского набора настроек.
        /// Используется в окне настроек для сброса к заводским настройкам.
        /// Не меняет конфигурации сами по себе, а только создает копию.
        /// </summary>
        /// <returns></returns>
        public ConfigDto GetDefaultConfig();
    }

    public class ConfigService : IConfigService
    {
        private readonly IConfigLoader _loader;
        private readonly SemaphoreSlim _lock = new(1, 1);

        /// <summary>
        /// Локальные настройки
        /// </summary>
        private LocalConfig? _LOCAL;
        /// <summary>
        /// Глобальные настройки
        /// </summary>
        private readonly AsyncLazy<GlobalConfig> _GLOBAL;

        public string RootDirectory { get; private set; } = string.Empty;

        public ConfigService(IConfigLoader loader)
        {
            _loader = loader;
            _GLOBAL = new(() => _loader.LoadGlobal());
        }

        public async Task<ConfigDto> GetConfigAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var global = await _GLOBAL.GetValueAsync();

                return new ConfigDto(
                    new GlobalConfigDto(global),
                    _LOCAL is null ? new LocalConfigDto() : new LocalConfigDto(_LOCAL)
                    );
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SaveConfigAsync(ConfigDto allConfig)
        {
            await _lock.WaitAsync();
            try
            {
                var globalConfig = new GlobalConfig(allConfig.Global);
                await _loader.SaveGlobal(globalConfig);

                // Сброс ленивой загрузки для глобальной конфигурации
                _GLOBAL.Reset(globalConfig);

                if (allConfig.Local != null)
                {
                    var localConfig = new LocalConfig(allConfig.Local);
                    await _loader.SaveLocal(localConfig, RootDirectory);
                    _LOCAL = localConfig;
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task OpenProjectAsync(string rootDirectory)
        {
            await _lock.WaitAsync();
            try
            {
                RootDirectory = rootDirectory;
                _LOCAL = await _loader.LoadLocal(RootDirectory);

            }
            finally
            {
                _lock.Release();
            }
        }

        public ConfigDto GetDefaultConfig()
        {
            return new ConfigDto(new GlobalConfigDto(new GlobalConfig()), new LocalConfigDto(new LocalConfig()));
        }
    }
}
