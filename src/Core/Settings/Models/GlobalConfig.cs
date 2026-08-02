using Core.Settings.Models.DTO;

namespace Core.Settings.Models
{
    /// <summary>
    /// Модель глобальных настроек
    /// </summary>
    public class GlobalConfig : BaseConfig
    {
        // Документация
        public string DocsTemplate { get; set; } = "";


        public GlobalConfig() { }
        public GlobalConfig(GlobalConfigDto configDto)
        {
            ContextKey = configDto.ContextKey;
            QueryKey = configDto.QueryKey;
            FilePathKey = configDto.FilePathKey;
            FileContentKey = configDto.FileContentKey;
            FileStructure = configDto.FileStructure;

            DocsTemplate = configDto.DocsTemplate;
        }
    }
}
