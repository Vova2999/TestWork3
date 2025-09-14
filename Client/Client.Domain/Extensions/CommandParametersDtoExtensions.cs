using System.Reflection;
using Client.Domain.Attributes;
using Client.Domain.Dtos;

namespace Client.Domain.Extensions;

public static class CommandParametersDtoExtensions
{
    private static readonly IDictionary<Type, string> ParametersToCommand;

    static CommandParametersDtoExtensions()
    {
        ParametersToCommand = typeof(CommandParametersDto).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(CommandParametersDto)))
            .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<CommandParametersForAttribute>() })
            .Where(typeWithAttribute => typeWithAttribute.Attribute != null)
            .ToDictionary(typeWithAttribute => typeWithAttribute.Type, typeWithAttribute => typeWithAttribute.Attribute!.Command);
    }

    public static CommandDto ToCommandDto(this CommandParametersDto commandParametersDto)
    {
        return new CommandDto
        {
            Command = ParametersToCommand.TryGetValue(commandParametersDto.GetType(), out var command) ? command : null!,
            CommandParameters = commandParametersDto
        };
    }
}