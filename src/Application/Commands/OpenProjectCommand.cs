using Application.Interfaces;
using MediatR;

namespace Application.Commands
{

    public record OpenProjectCommand(string RootDirectory) : IRequest<int>;

    public class OpenProjectCommandHandler(IConfigService cs, ICatalogObserver observer) : IRequestHandler<OpenProjectCommand, int>
    {
        public async Task<int> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            await cs.SetRootDirectoryAsync(command.RootDirectory);
            observer.StartObserving();
            return 0;
        }
    }
}
