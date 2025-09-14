using Server.Domain.Dtos;
using Server.Services.CommandParameters;

namespace Server.Services.Command;

public class CommandHandler : ICommandHandler
{
    private readonly IDictionary<Type, ICommandParametersHandler> _commandParametersHandlers;

    public CommandHandler(IEnumerable<ICommandParametersHandler> commandParametersHandlers)
    {
        _commandParametersHandlers = commandParametersHandlers.ToDictionary(handler => handler.CommandParametersDtoType);
    }

    public async Task<CommandResultDto> HandleAsync(CommandDto command)
    {
        if (command.CommandParameters != null && _commandParametersHandlers.TryGetValue(command.CommandParameters.GetType(), out var handler))
            return await handler.HandleAsync(command.CommandParameters);

        return new CommandResultDto
        {
            Command = command.Command,
            Success = false,
            ErrorMessage = $"Not found handler for command {command.Command}"
        };
    }
}