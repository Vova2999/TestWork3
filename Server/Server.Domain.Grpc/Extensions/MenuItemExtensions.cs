using Sms.Test;

namespace Server.Domain.Grpc.Extensions;

public static class MenuItemExtensions
{
    public static MenuItem ToGrpcDto(this Entities.MenuItem menuItem)
    {
        return new MenuItem
        {
            Id = menuItem.Id,
            Article = menuItem.Article,
            Name = menuItem.Name,
            Price = (double) menuItem.Price,
            IsWeighted = menuItem.IsWeighted,
            FullPath = menuItem.FullPath,
            Barcodes = { menuItem.Barcodes }
        };
    }
}