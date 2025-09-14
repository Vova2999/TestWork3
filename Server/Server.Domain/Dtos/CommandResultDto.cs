#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Server.Domain.Dtos;

public class CommandResultDto
{
    public string? Command { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public CommandResultDataDto? Data { get; set; }
}