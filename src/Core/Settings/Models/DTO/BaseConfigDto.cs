using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Активные настройки
    /// </summary>
    public class BaseConfigDto: BaseConfig
    {
        public BaseConfigDto(LocalConfigDto? local, GlobalConfigDto global)
        {
            BaseConfig currentConfig = local is null ? global : local;

            IntroductionKey = currentConfig.IntroductionKey;
            ContextKey = currentConfig.ContextKey;
            RulesKey = currentConfig.RulesKey;
            QueryKey = currentConfig.QueryKey;
            FilePathKey = currentConfig.FilePathKey;
            FileContentKey = currentConfig.FileContentKey;
            PromptStructure = currentConfig.PromptStructure;
            FileStructure = currentConfig.FileStructure;
            IntroductionText = currentConfig.IntroductionText;
            RulesText = currentConfig.RulesText;
        }
    }
}
