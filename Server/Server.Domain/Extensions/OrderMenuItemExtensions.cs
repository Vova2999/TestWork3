using Server.Domain.Dtos;
using Server.Domain.Entities;

namespace Server.Domain.Extensions;

public static class OrderMenuItemExtensions
{
    public static OrderMenuItemDto ToDto(this OrderMenuItem orderMenuItem)
    {
        return new OrderMenuItemDto
        {
            Id = orderMenuItem.MenuItemId,
            Quantity = orderMenuItem.Quantity
        };
    }
}