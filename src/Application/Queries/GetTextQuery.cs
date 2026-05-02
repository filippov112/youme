using Application.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetTextQuery(string Path) : IRequest<string>;

    public class GetTextQueryHandler(IFileSystemManager fsm) : IRequestHandler<GetTextQuery, string>
    {
        public async Task<string> Handle(GetTextQuery request, CancellationToken cancellationToken)
        {
            return await fsm.ReadFileAsync(request.Path);
        }
    }
}
