using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Commands
{

    public record SaveSettingsCommand(AllConfigDto Config) : IRequest<int>;

    public class SaveSettingsCommandHandler(IConfigService cs) : IRequestHandler<SaveSettingsCommand, int>
    {
        public async Task<int> Handle(SaveSettingsCommand command, CancellationToken cancellationToken)
        {
            await cs.SaveAllConfigAsync(command.Config);
            return 0;
        }
    }
}
