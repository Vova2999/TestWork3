using Client.Api.Clients.Menu;
using Client.Domain.Dtos;
using Client.Domain.Dtos.CommandParameters;
using Client.Domain.Dtos.CommandResultData;
using Client.Domain.Extensions;

namespace Client.Services.Menu;

public class MenuService : IMenuService
{
    private readonly IMenuClient _menuClient;

    public MenuService(IMenuClient menuClient)
    {
        _menuClient = menuClient;
    }

    public async Task<GetMenuCommandResultDataDto?> GetMenuAsync(bool withPrice)
    {
        var command = new GetMenuCommandParametersDto { WithPrice = withPrice }.ToCommandDto();

        var commandResult = await _menuClient.HandleAsync(command);
        return commandResult.Success
            ? commandResult.Data as GetMenuCommandResultDataDto
            : throw new InvalidOperationException("Error on get menu");
    }

    public async Task<SendOrderCommandResultDataDto?> SendOrderAsync(Guid orderId, ICollection<OrderMenuItemDto> menuItems)
    {
        var command = new SendOrderCommandParametersDto { OrderId = orderId, MenuItems = menuItems }.ToCommandDto();

        var commandResult = await _menuClient.HandleAsync(command);
        return commandResult.Success
            ? commandResult.Data as SendOrderCommandResultDataDto
            : throw new InvalidOperationException("Error on send order");
    }
}