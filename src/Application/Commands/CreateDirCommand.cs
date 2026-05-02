using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public record CreateDirCommand(string Path) : IRequest<int>;

    public class CreateDirCommandHandler(IFileSystemManager fsm) : IRequestHandler<CreateDirCommand, int>
    {
        public async Task<int> Handle(CreateDirCommand command, CancellationToken token)
        {
            await fsm.CreateDirectoryAsync(command.Path);
            return 0;
        }
    }
}
