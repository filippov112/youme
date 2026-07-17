using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class Prompt(string structure, Rules rules, Query query, Introduction introduction, Context context) :
        ComplexBlock(structure, [rules, query, introduction, context])
    {
    }
}
