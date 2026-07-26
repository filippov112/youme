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
            var config = await cs.GetConfigAsync();
            var activeSettings = config.GetActiveSettings();
            List<File> files = [];
            foreach (var path in filePath)
            {
                string? content = await fsm.ReadFileAsync(path);
                var file = new File(
                    activeSettings.FileStructure,
                    new FilePath(activeSettings.FilePathKey, path),
                    new FileContent(activeSettings.FileContentKey, content)
                    );
                files.Add(file);
            }
            var context = new Context(activeSettings.ContextKey, files);
            var intro = new Introduction(activeSettings.IntroductionKey, activeSettings.IntroductionText);
            var rules = new Rules(activeSettings.RulesKey, activeSettings.RulesText);
            var query = new Query(activeSettings.QueryKey, queryText);
            var prompt = new Prompt(activeSettings.PromptStructure, rules, query, intro, context);

            string result = prompt.Build();
            return result;
        }
    }
}
