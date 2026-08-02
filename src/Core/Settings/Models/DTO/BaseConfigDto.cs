namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Активные настройки
    /// </summary>
    public class BaseConfigDto : BaseConfig
    {
        public BaseConfigDto(LocalConfigDto? local, GlobalConfigDto global)
        {
            BaseConfig currentConfig = local is null ? global : local;

            ContextKey = currentConfig.ContextKey;
            QueryKey = currentConfig.QueryKey;
            FilePathKey = currentConfig.FilePathKey;
            FileContentKey = currentConfig.FileContentKey;
            FileStructure = currentConfig.FileStructure;
        }
    }
}
