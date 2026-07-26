using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Глобальные настройки
    /// </summary>
    public class GlobalConfigDto: GlobalConfig
    {
        public GlobalConfigDto() { }
        public GlobalConfigDto(GlobalConfig config)
        {
            IntroductionKey = config.IntroductionKey;
            ContextKey = config.ContextKey;
            RulesKey = config.RulesKey;
            QueryKey = config.QueryKey;
            FilePathKey = config.FilePathKey;
            FileContentKey = config.FileContentKey;
            PromptStructure = config.PromptStructure;
            FileStructure = config.FileStructure;
            IntroductionText = config.IntroductionText;
            RulesText = config.RulesText;

            DocsTemplate = config.DocsTemplate;
        }

        public GlobalConfigDto GetCopy() => new(new(this));
    }
}
