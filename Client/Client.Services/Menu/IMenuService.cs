using Client.Domain.Dtos;
using Client.Domain.Dtos.CommandResultData;

namespace Client.Services.Menu;

public interface IMenuService
{
    Task<GetMenuCommandResultDataDto?> GetMenuAsync(bool withPrice);
    Task<SendOrderCommandResultDataDto?> SendOrderAsync(Guid orderId, ICollection<OrderMenuItemDto> menuItems);
}