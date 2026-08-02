namespace Core.Settings.Models.DTO
{
    /// <summary>
    /// Локальные настройки
    /// </summary>
    public class LocalConfigDto : LocalConfig
    {
        public LocalConfigDto() { }
        public LocalConfigDto(LocalConfig config)
        {
            ContextKey = config.ContextKey;
            QueryKey = config.QueryKey;
            FilePathKey = config.FilePathKey;
            FileContentKey = config.FileContentKey;
            FileStructure = config.FileStructure;
        }

        public LocalConfigDto GetCopy() => new(new(this));
    }
}
