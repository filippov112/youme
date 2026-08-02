namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Модель окна настроек
    /// </summary>
    public record ConfigDto
    {
        /// <summary>
        /// Конфигурация отдельного проекта.
        /// </summary>
        public LocalConfigDto? Local { get; set; }
        /// <summary>
        /// Глобальная конфигурация приложения.
        /// </summary>
        public GlobalConfigDto Global { get; set; } = new();

        public ConfigDto(GlobalConfig globalConfig, LocalConfig localConfig)
        {
            Global = new(globalConfig);
            Local = new(localConfig);
        }

        /// <summary>
        /// Для тестов
        /// </summary>
        public ConfigDto()
        {
            Local = new();
        }
        /// <summary>
        /// Получить текущие значения параметров с переопределением
        /// </summary>
        /// <returns></returns>
        public BaseConfigDto GetActiveSettings() => new(Local, Global);
    }
}
