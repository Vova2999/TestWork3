#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Server.Domain.Attributes;

namespace Server.Domain.Dtos.CommandResultData;

[CommandResultDataFor("SendOrder")]
public class SendOrderCommandResultDataDto : CommandResultDataDto
{
}