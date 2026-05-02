namespace Application.Models
{
    public record CombinationConfig
    {
        // Ключи
        public string IntroductionKey { get; set; } = "##intro##";
        public string ContextKey { get; set; } = "##context##";
        public string RulesKey { get; set; } = "##rules##";
        public string QueryKey { get; set; } = "##query##";
        public string FilePathKey { get; set; } = "##path##";
        public string FileContentKey { get; set; } = "##content##";

        // Структуры
        public string PromptStructure { get; set; } = "";
        public string FileStructure { get; set; } = "";

        // Значения по умолчанию
        public string IntroductionText { get; set; } = "";
        public string RulesText { get; set; } = "";

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
