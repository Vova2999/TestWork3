using Server.Domain.Dtos;
using Server.Domain.Entities;

namespace Server.Domain.Extensions;

public static class MenuItemExtensions
{
    public static MenuItemDto ToDto(this MenuItem menuItem)
    {
        return new MenuItemDto
        {
            Id = menuItem.Id,
            Article = menuItem.Article,
            Name = menuItem.Name,
            Price = menuItem.Price,
            IsWeighted = menuItem.IsWeighted,
            FullPath = menuItem.FullPath,
            Barcodes = menuItem.Barcodes
        };
    }
}