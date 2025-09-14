#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Server.Domain.Attributes;

namespace Server.Domain.Dtos.CommandParameters;

[CommandParametersFor("GetMenu")]
public class GetMenuCommandParametersDto : CommandParametersDto
{
    public bool WithPrice { get; set; }
}