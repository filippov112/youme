namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Глобальные настройки
    /// </summary>
    public class GlobalConfigDto : GlobalConfig
    {
        public GlobalConfigDto() { }
        public GlobalConfigDto(GlobalConfig config)
        {
            ContextKey = config.ContextKey;
            QueryKey = config.QueryKey;
            FilePathKey = config.FilePathKey;
            FileContentKey = config.FileContentKey;
            FileStructure = config.FileStructure;

            DocsTemplate = config.DocsTemplate;
        }

        public GlobalConfigDto GetCopy() => new(new(this));
    }
}
