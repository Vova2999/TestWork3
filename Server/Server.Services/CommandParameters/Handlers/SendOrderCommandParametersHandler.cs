using Server.Domain.Dtos.CommandParameters;
using Server.Domain.Dtos.CommandResultData;

namespace Server.Services.CommandParameters.Handlers;

public class SendOrderCommandParametersHandler : CommandParametersHandlerBase<SendOrderCommandParametersDto, SendOrderCommandResultDataDto>
{
    protected override Task<SendOrderCommandResultDataDto> HandleInternalAsync(SendOrderCommandParametersDto commandParameters)
    {
        return Task.FromResult(new SendOrderCommandResultDataDto());
    }
}