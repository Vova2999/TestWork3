#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Client.Domain.Attributes;

namespace Client.Domain.Dtos.CommandParameters;

[CommandParametersFor("GetMenu")]
public class GetMenuCommandParametersDto : CommandParametersDto
{
    public bool WithPrice { get; set; }
}