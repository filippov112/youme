using Application.Interfaces;
using MediatR;

namespace Application.Commands
{
    public record WriteFileCommand(string Path, string Content) : IRequest<int>;

    public class WriteFileCommandHandler(IFileSystemManager fsm) : IRequestHandler<WriteFileCommand, int>
    {
        public async Task<int> Handle(WriteFileCommand command, CancellationToken token)
        {
            await fsm.WriteFileAsync(command.Path, command.Content);
            return 0;
        }
    }
}
