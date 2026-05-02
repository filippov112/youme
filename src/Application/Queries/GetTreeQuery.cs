using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Queries
{
    public record GetTreeQuery() : IRequest<ProjectUnit?>;

    public class GetTreeQueryHandler(IConfigService cs, IFileSystemManager fsm) : IRequestHandler<GetTreeQuery, ProjectUnit?>
    {
        public async Task<ProjectUnit?> Handle(GetTreeQuery request, CancellationToken cancellationToken)
        {
            return await fsm.GetTreeAsync(cs.RootDirectory);
        }
    }
}
