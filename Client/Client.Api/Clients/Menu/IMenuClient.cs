using Client.Domain.Dtos;

namespace Client.Api.Clients.Menu;

public interface IMenuClient
{
    Task<CommandResultDto> HandleAsync(CommandDto commandDto, CancellationToken cancellationToken = default);
}