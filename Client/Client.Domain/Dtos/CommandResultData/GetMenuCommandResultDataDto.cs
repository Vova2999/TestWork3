#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Client.Domain.Attributes;

namespace Client.Domain.Dtos.CommandResultData;

[CommandResultDataFor("GetMenu")]
public class GetMenuCommandResultDataDto : CommandResultDataDto
{
    public ICollection<MenuItemDto> MenuItems { get; set; }
}