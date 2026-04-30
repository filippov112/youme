using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class Config
    {
        // Ключи
        public string IntroductionKey { get; set; } = "##intro##";
        public string ContextKey { get; set; } = "##context##";
        public string RulesKey { get; set; } = "##rules##";
        public string QueryKey { get; set; } = "##query##";
        public string FilePathKey { get; set; } = "##path##";
        public string FileContentKey { get; set; } = "##content##";

        // Структуры
        public string PromptStructure { get; set; }
        public string FileStructure { get; set; }

        // Значения по умолчанию
        public string IntroductionDef { get; set; } = "";
        public string RulesDef { get; set; } = "";
            
        public Config()
        {
            PromptStructure = $"{IntroductionKey}\n`````\n{ContextKey}\n`````\n{QueryKey}\n\n{RulesKey}";
            FileStructure = $"Файл: {FilePathKey}\n````\n{FileContentKey}\n````";
        }
    }
}
