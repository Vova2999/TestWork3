#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Client.Domain.Attributes;

namespace Client.Domain.Dtos.CommandParameters;

[CommandParametersFor("SendOrder")]
public class SendOrderCommandParametersDto : CommandParametersDto
{
    public Guid OrderId { get; set; }
    public ICollection<OrderMenuItemDto> MenuItems { get; set; }
}