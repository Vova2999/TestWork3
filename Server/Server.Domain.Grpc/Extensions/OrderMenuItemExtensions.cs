using Server.Domain.Entities;
using Sms.Test;

namespace Server.Domain.Grpc.Extensions;

public static class OrderMenuItemExtensions
{
    public static OrderItem ToGrpcDto(this OrderMenuItem orderMenuItem)
    {
        return new OrderItem
        {
            Id = orderMenuItem.MenuItemId,
            Quantity = (double) orderMenuItem.Quantity
        };
    }
}