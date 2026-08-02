using Core.Settings.Models.DTO;

namespace Core.Settings.Models
{
    /// <summary>
    /// Модель локальных настроек
    /// </summary>
    public class LocalConfig : BaseConfig
    {
        public LocalConfig() { }
        public LocalConfig(LocalConfigDto configDto)
        {
            ContextKey = configDto.ContextKey;
            QueryKey = configDto.QueryKey;
            FilePathKey = configDto.FilePathKey;
            FileContentKey = configDto.FileContentKey;
            FileStructure = configDto.FileStructure;

        }
    }
}
