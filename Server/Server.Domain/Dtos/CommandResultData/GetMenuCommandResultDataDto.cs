#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Server.Domain.Attributes;

namespace Server.Domain.Dtos.CommandResultData;

[CommandResultDataFor("GetMenu")]
public class GetMenuCommandResultDataDto : CommandResultDataDto
{
    public ICollection<MenuItemDto> MenuItems { get; set; }
}