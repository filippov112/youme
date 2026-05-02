namespace Application.Models
{
    public record AllConfigDto
    {
        public CombinationConfig? Local { get; set; }
        public CombinationConfig Global { get; set; } = new();
    }
}
