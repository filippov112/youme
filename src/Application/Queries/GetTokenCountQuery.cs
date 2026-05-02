using Application.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetTokenCountQuery(string Prompt) : IRequest<int>;
    public class GetTokenCountQueryHandler(ITokenCounter counter) : IRequestHandler<GetTokenCountQuery, int>
    {
        public async Task<int> Handle(GetTokenCountQuery request, CancellationToken cancellationToken)
        {
            return counter.CalcTokenCount(request.Prompt);
        }
    }
}
