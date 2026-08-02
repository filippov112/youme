using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public record Query(string Key, string Value) : PromptComponent(Key, Value);
}
