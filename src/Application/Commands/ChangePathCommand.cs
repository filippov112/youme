using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public record ChangePathCommand(string OldPath, string NewPath) : IRequest<int>;

    public class ChangePathCommandHandler(IFileSystemManager fsm) : IRequestHandler<ChangePathCommand, int>
    {
        public async Task<int> Handle(ChangePathCommand command, CancellationToken cancellationToken)
        {
            await fsm.ChangePathAsync(command.OldPath, command.NewPath);
            return 0;
        }
    }
}
