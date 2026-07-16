namespace Pdp.App.Models
{
    public record CombinationConfig
    {
        // Условные обозначения ключей для замены в структурах промпта.
        public string IntroductionKey { get; set; } = "##intro##"; // Блок введения
        public string ContextKey { get; set; } = "##context##"; // Контекстный блок
        public string RulesKey { get; set; } = "##rules##"; // Блок правил
        public string QueryKey { get; set; } = "##query##"; // Блок запроса
        public string FilePathKey { get; set; } = "##path##"; // Расположение файла
        public string FileContentKey { get; set; } = "##content##"; // Содержимое файла

        // Структуры
        public string PromptStructure { get; set; } = ""; // Структура промпта
        public string FileStructure { get; set; } = ""; // Структура отдельного файла в блоке контекста

        // Значения по умолчанию
        public string IntroductionText { get; set; } = ""; // Текст блока введения
        public string RulesText { get; set; } = ""; // Текст блока правил

        /// <summary>
        /// Конструктор для маппинга из доменной сущности.
        /// </summary>
        /// <param name="current"></param>
        public CombinationConfig(Config current)
        {
            IntroductionKey = current.IntroductionKey;
            ContextKey = current.ContextKey;
            RulesKey = current.RulesKey;
            QueryKey = current.QueryKey;
            FilePathKey = current.FilePathKey;
            FileContentKey = current.FileContentKey;

            // Структуры
            PromptStructure = current.PromptStructure;
            FileStructure = current.FileStructure;

            // Значения по умолчанию
            IntroductionText = current.IntroductionDef;
            RulesText = current.RulesDef;
        }
        public CombinationConfig() { }
    }
}
