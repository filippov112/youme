using Domain.Models.Other;
using Domain.Models.PromptComponents;

namespace Domain.Models
{
    public class Prompt(string structure, Rules rules, Query query, Introduction introduction, Context context) :
        ComplexBlock(structure, [rules, query, introduction, context])
    {
    }
}
