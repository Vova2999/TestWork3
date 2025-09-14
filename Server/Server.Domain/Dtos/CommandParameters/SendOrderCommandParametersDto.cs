#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Server.Domain.Attributes;

namespace Server.Domain.Dtos.CommandParameters;

[CommandParametersFor("SendOrder")]
public class SendOrderCommandParametersDto : CommandParametersDto
{
    public Guid OrderId { get; set; }
    public List<OrderMenuItemDto> MenuItems { get; set; }
}