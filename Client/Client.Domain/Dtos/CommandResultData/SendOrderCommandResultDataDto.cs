#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Client.Domain.Attributes;

namespace Client.Domain.Dtos.CommandResultData;

[CommandResultDataFor("SendOrder")]
public class SendOrderCommandResultDataDto : CommandResultDataDto
{
}