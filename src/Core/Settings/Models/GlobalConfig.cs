using Core.Settings.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models
{
    /// <summary>
    /// Модель глобальных настроек
    /// </summary>
    public class GlobalConfig: BaseConfig
    {
        // Документация
        public string DocsTemplate { get; set; } = "";


        public GlobalConfig() { }
        public GlobalConfig(GlobalConfigDto configDto)
        {
            IntroductionKey = configDto.IntroductionKey;
            ContextKey = configDto.ContextKey;
            RulesKey = configDto.RulesKey;
            QueryKey = configDto.QueryKey;
            FilePathKey = configDto.FilePathKey;
            FileContentKey = configDto.FileContentKey;
            PromptStructure = configDto.PromptStructure;
            FileStructure = configDto.FileStructure;
            IntroductionText = configDto.IntroductionText;
            RulesText = configDto.RulesText;

            DocsTemplate = configDto.DocsTemplate;
        }
    }
}
