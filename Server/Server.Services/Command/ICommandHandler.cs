using Server.Domain.Dtos;

namespace Server.Services.Command;

public interface ICommandHandler
{
    Task<CommandResultDto> HandleAsync(CommandDto? command);
}