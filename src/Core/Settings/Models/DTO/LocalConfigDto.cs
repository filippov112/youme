using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Локальные настройки
    /// </summary>
    public class LocalConfigDto: LocalConfig
    {
        public LocalConfigDto() { }
        public LocalConfigDto(LocalConfig config)
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
        }

        public LocalConfigDto GetCopy() => new(new(this));
    }
}
