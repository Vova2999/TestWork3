#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Server.Domain.Dtos;

public class CommandDto
{
    public string Command { get; set; }
    public CommandParametersDto? CommandParameters { get; set; }
}