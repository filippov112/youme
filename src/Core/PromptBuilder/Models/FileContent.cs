using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public record FileContent(string Key, string Content) : PromptComponent(Key, Content);
}
