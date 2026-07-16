using Pdp.App.Models;

namespace Pdp.App.Interfaces
{
    public interface IConfigService
    {
        /// <summary>
        /// Корневой каталог открытого проекта
        /// </summary>
        public string RootDirectory { get; }

        
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
}
