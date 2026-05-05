using Pdp.App.Interfaces;
using Pdp.App.Models;
using Pdp.Domain.Models;
using Pdp.Domain.Models.PromptComponents;
using Pdp.Domain.Models.PromptComponents.FileComponents;
using File = Pdp.Domain.Models.PromptComponents.File;

namespace Pdp.App.Services
{
    public class PromptBuilder(IConfigService cs, IFileSystemManager fsm) : IPromptBuilder
    {
        public async Task<string> GetPrompt(List<string> filePath, string queryText)
        {
            CombinationConfig config = await cs.GetCombinationConfigAsync();
            List<File> files = [];
            foreach (var path in filePath)
            {
                string? content = await fsm.ReadFileAsync(path);
                var file = new File(
                    config.FileStructure,
                    new FilePath(config.FilePathKey, path),
                    new FileContent(config.FileContentKey, content)
                    );
                files.Add(file);
            }
            var context = new Context(config.ContextKey, files);
            var intro = new Introduction(config.IntroductionKey, config.IntroductionText);
            var rules = new Rules(config.RulesKey, config.RulesText);
            var query = new Query(config.QueryKey, queryText);
            var prompt = new Prompt(config.PromptStructure, rules, query, intro, context);

            string result = prompt.Build();
            return result;
        }
    }
}
