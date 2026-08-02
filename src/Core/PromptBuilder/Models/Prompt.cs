using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class Prompt(string layout, Query query, Context context, Dictionary<string, string> fileComponents) :
        ComplexBlock(layout, [query, context, .. fileComponents.Select((item) => new PromptComponent(item.Key, item.Value))])
    {
    }
}
