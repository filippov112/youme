using Core.Settings.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models
{
    /// <summary>
    /// Модель локальных настроек
    /// </summary>
    public class LocalConfig: BaseConfig
    {
        public LocalConfig() { }
        public LocalConfig(LocalConfigDto configDto)
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

        }
    }
}
