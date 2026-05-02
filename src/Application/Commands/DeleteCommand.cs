using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public record DeleteCommand(string Path) : IRequest<int>;

    public class DeleteCommandHandler(IFileSystemManager fsm): IRequestHandler<DeleteCommand, int>
    {
        public async Task<int> Handle(DeleteCommand command, CancellationToken token)
        {
            await fsm.DeleteAsync(command.Path);
            return 0;
        }
    }
}
