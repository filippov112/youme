using Application.Interfaces;
using Application.Models;
using Domain.Models;
using Domain.Models.PromptComponents;
using Domain.Models.PromptComponents.FileComponents;
using MediatR;
using File = Domain.Models.PromptComponents.File;

namespace Application.Queries
{
    public record GetPromptQuery(List<string> FilePaths, string Query) : IRequest<string>;

    public class GetPromptQueryHandler(IConfigService cs, IFileSystemManager fsm) : IRequestHandler<GetPromptQuery, string>
    {
        public async Task<string> Handle(GetPromptQuery request, CancellationToken cancellationToken)
        {
            CombinationConfig config = await cs.GetCombinationConfigAsync();
            List<File> files = [];
            foreach (var path in request.FilePaths)
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
            var query = new Query(config.QueryKey, request.Query);
            var prompt = new Prompt(config.PromptStructure, rules, query, intro, context);

            string result = prompt.Build();
            return result;
        }
    }
}
