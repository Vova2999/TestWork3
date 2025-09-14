using Server.Domain.Dtos;
using Server.Domain.Extensions;

namespace Server.Services.CommandParameters;

public abstract class CommandParametersHandlerBase<TCommandParametersDto, TCommandResultDataDto> : ICommandParametersHandler
    where TCommandParametersDto : CommandParametersDto
    where TCommandResultDataDto : CommandResultDataDto
{
    public Type CommandParametersDtoType => typeof(TCommandParametersDto);

    public async Task<CommandResultDto> HandleAsync(CommandParametersDto commandParameters)
    {
        var commandResultData = await HandleInternalAsync(commandParameters as TCommandParametersDto
            ?? throw new InvalidOperationException($"{nameof(commandParameters)} in not {typeof(TCommandParametersDto).Name}"));

        return commandResultData.ToSuccessResultDto();
    }

    protected abstract Task<TCommandResultDataDto> HandleInternalAsync(TCommandParametersDto commandParameters);
}