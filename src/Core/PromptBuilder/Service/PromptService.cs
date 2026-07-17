using Core.Explorer.Services;
using Core.PromptBuilder.Models;
using Core.Settings.Models;
using Core.Settings.Services;
using File = Core.PromptBuilder.Models.File;

namespace Core.PromptBuilder.Service
{
    public interface IPromptService
    {
        public Task<string> GetPrompt(List<string> filePath, string queryText);
    }
    public class PromptService(IConfigService cs, IFileSystemService fsm) : IPromptService
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
