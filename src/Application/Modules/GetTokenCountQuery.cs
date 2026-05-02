using Application.Interfaces;
using MediatR;

namespace Application.Modules
{
    public record GetTokenCountQuery(string prompt) : IRequest<int>;
    public class GetTokenCountQueryHandler(ITokenCounter counter) : IRequestHandler<GetTokenCountQuery, int>
    {
        public async Task<int> Handle(GetTokenCountQuery request, CancellationToken cancellationToken)
        {
            return counter.CalcTokenCount(request.prompt);
        }
    }
}
