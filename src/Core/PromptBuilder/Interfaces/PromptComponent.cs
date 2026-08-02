namespace Core.PromptBuilder.Interfaces
{
    public record PromptComponent(string Key, string Value) : IComponent;
}
