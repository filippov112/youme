namespace Pdp.App.Models
{
    public record AllConfigDto
    {
        /// <summary>
        /// Конфигурация отдельного проекта.
        /// </summary>
        public CombinationConfig? Local { get; set; }
        /// <summary>
        /// Глобальная конфигурация приложения.
        /// </summary>
        public CombinationConfig Global { get; set; } = new();
    }
}
