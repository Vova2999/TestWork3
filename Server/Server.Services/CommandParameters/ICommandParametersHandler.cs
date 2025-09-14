using Server.Domain.Dtos;

namespace Server.Services.CommandParameters;

public interface ICommandParametersHandler
{
    Type CommandParametersDtoType { get; }
    Task<CommandResultDto> HandleAsync(CommandParametersDto commandParameters);
}