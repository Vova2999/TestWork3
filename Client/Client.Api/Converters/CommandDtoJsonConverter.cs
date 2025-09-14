using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Client.Api.Helpers;
using Client.Common.Extensions;
using Client.Domain.Attributes;
using Client.Domain.Dtos;

namespace Client.Api.Converters;

public class CommandDtoJsonConverter : JsonConverter<CommandDto>
{
    private static readonly IDictionary<string, Type> CommandToParametersType;
    private static readonly ConcurrentDictionary<(string ProprtyName, JsonNamingPolicy? NamingPolicy), string> CommandDtoPropertyNamesCache;

    static CommandDtoJsonConverter()
    {
        CommandToParametersType = typeof(CommandParametersDto).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(CommandParametersDto)))
            .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<CommandParametersForAttribute>() })
            .Where(typeWithAttribute => typeWithAttribute.Attribute != null)
            .ToDictionary(typeWithAttribute => typeWithAttribute.Attribute!.Command.ToLower(), typeWithAttribute => typeWithAttribute.Type);

        CommandDtoPropertyNamesCache = new ConcurrentDictionary<(string, JsonNamingPolicy?), string>();
    }

    private static string GetCommandDtoPropertyName(string propertyName, JsonNamingPolicy? namingPolicy)
    {
        return CommandDtoPropertyNamesCache.GetOrAdd(
            (propertyName, namingPolicy),
            props => JsonConverterHelper.GetPropertyName(typeof(CommandDto), props.ProprtyName, props.NamingPolicy));
    }

    public override CommandDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var (commandDto, parametersNode) = ReadWithoutCommandParameters(ref reader, options);

            if (commandDto == null || parametersNode == null || commandDto.Command.IsNullOrEmpty())
                return commandDto;

            if (!CommandToParametersType.TryGetValue(commandDto.Command.ToLower(), out var parametersType))
                return commandDto;

            commandDto.CommandParameters = parametersNode.Deserialize(parametersType, options) as CommandParametersDto;
            return commandDto;
        }
        catch
        {
            return null;
        }
    }

    private (CommandDto? CommandDto, JsonNode? ParametersNode) ReadWithoutCommandParameters(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var namingPolicy = options.PropertyNamingPolicy;

        var optionsWithoutThisConverter = new JsonSerializerOptions(options);
        optionsWithoutThisConverter.Converters.Remove(this);

        if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
            return (null, null);

        const string commandParametersName = nameof(CommandDto.CommandParameters);
        var commandParametersPropertyName = GetCommandDtoPropertyName(commandParametersName, namingPolicy);

        var parametersNode = jsonObject[commandParametersPropertyName];
        jsonObject.Remove(commandParametersPropertyName);

        var commandDto = jsonObject.Deserialize<CommandDto>(optionsWithoutThisConverter);
        return (commandDto, parametersNode);
    }

    public override void Write(Utf8JsonWriter writer, CommandDto? value, JsonSerializerOptions options)
    {
        try
        {
            var namingPolicy = options.PropertyNamingPolicy;

            var jsonObject = WriteWithoutCommandParameters(value, options);
            if (jsonObject == null)
                return;

            const string commandParametersName = nameof(CommandDto.CommandParameters);
            var commandParametersPropertyName = GetCommandDtoPropertyName(commandParametersName, namingPolicy);

            if (jsonObject[commandParametersPropertyName] != null && value?.CommandParameters != null)
                jsonObject[commandParametersPropertyName] = JsonSerializer.SerializeToNode(value.CommandParameters, value.CommandParameters.GetType(), options);

            jsonObject.WriteTo(writer);
        }
        catch
        {
            // ignored
        }
    }

    private JsonNode? WriteWithoutCommandParameters(CommandDto? value, JsonSerializerOptions options)
    {
        var optionsWithoutConverter = new JsonSerializerOptions(options);
        optionsWithoutConverter.Converters.Remove(this);

        return JsonSerializer.SerializeToNode(value, optionsWithoutConverter);
    }
}