namespace Core.Settings.Models
{
    /// <summary>
    /// Базовая модель конфигурации.
    /// Содержит переопределяемые параметры
    /// </summary>
    public abstract class BaseConfig
    {
        // Условные обозначения ключей для замены в структурах промпта.
        public string ContextKey { get; set; } = "##context##"; // Контекстный блок
        public string QueryKey { get; set; } = "##query##"; // Блок запроса
        public string FilePathKey { get; set; } = "##path##"; // Расположение файла
        public string FileContentKey { get; set; } = "##content##"; // Содержимое файла

        // Структуры
        public string FileStructure = $"File: ##path##\n````\n##content##\n````"; // Структура отдельного файла в блоке контекста
    }
}
