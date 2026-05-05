using Pdp.Domain.Models.Other;
using Pdp.Domain.Models.PromptComponents;

namespace Pdp.Domain.Models
{
    public class Prompt(string structure, Rules rules, Query query, Introduction introduction, Context context) :
        ComplexBlock(structure, [rules, query, introduction, context])
    {
    }
}
