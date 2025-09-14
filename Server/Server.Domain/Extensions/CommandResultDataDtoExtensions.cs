using Server.Domain.Attributes;
using Server.Domain.Dtos;
using System.Reflection;

namespace Server.Domain.Extensions;

public static class CommandResultDataDtoExtensions
{
    private static readonly IDictionary<Type, string> ResultTypeToCommand;

    static CommandResultDataDtoExtensions()
    {
        ResultTypeToCommand = typeof(CommandResultDataDto).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(CommandResultDataDto)))
            .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<CommandResultDataForAttribute>() })
            .Where(typeWithAttribute => typeWithAttribute.Attribute != null)
            .ToDictionary(typeWithAttribute => typeWithAttribute.Type, typeWithAttribute => typeWithAttribute.Attribute!.Command);
    }

    public static CommandResultDto ToSuccessResultDto(this CommandResultDataDto commandResultDataDto)
    {
        return new CommandResultDto
        {
            Command = ResultTypeToCommand.TryGetValue(commandResultDataDto.GetType(), out var command) ? command : null!,
            Success = true,
            Data = commandResultDataDto
        };
    }
}