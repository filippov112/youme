using System;
using System.Collections.Generic;
using System.Text;

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
        public string IntroductionDef { get; set; } = "";
        public string RulesDef { get; set; } = "";

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
            IntroductionDef = current.IntroductionDef;
            RulesDef = current.RulesDef;
        }
        public CombinationConfig() { }
    }
}
