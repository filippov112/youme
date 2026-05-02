using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Queries
{
    public record GetFilteredTreeQuery(string Pattern) : IRequest<ProjectUnit?>;

    public class GetFilteredTreeQueryHandler(ISearchService search) : IRequestHandler<GetFilteredTreeQuery, ProjectUnit?>
    {
        public async Task<ProjectUnit?> Handle(GetFilteredTreeQuery request, CancellationToken cancellationToken)
        {
            return await search.FindMatchesAsync(request.Pattern);
        }
    }
}
