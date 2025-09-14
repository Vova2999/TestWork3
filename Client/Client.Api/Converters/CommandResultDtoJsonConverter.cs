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

public class CommandResultDtoJsonConverter : JsonConverter<CommandResultDto>
{
    private static readonly IDictionary<string, Type> CommandToResultDataType;
    private static readonly ConcurrentDictionary<(string ProprtyName, JsonNamingPolicy? NamingPolicy), string> CommandResultDataDtoPropertyNamesCache;

    static CommandResultDtoJsonConverter()
    {
        CommandToResultDataType = typeof(CommandResultDataDto).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(CommandResultDataDto)))
            .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<CommandResultDataForAttribute>() })
            .Where(typeWithAttribute => typeWithAttribute.Attribute != null)
            .ToDictionary(typeWithAttribute => typeWithAttribute.Attribute!.Command.ToLower(), typeWithAttribute => typeWithAttribute.Type);

        CommandResultDataDtoPropertyNamesCache = new ConcurrentDictionary<(string, JsonNamingPolicy?), string>();
    }

    private static string GetCommandResultDtoPropertyName(string propertyName, JsonNamingPolicy? namingPolicy)
    {
        return CommandResultDataDtoPropertyNamesCache.GetOrAdd(
            (propertyName, namingPolicy),
            props => JsonConverterHelper.GetPropertyName(typeof(CommandResultDto), props.ProprtyName, props.NamingPolicy));
    }

    public override CommandResultDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var (commandResultDto, dataNode) = ReadWithoutData(ref reader, options);

            if (commandResultDto == null || dataNode == null || commandResultDto.Command.IsNullOrEmpty())
                return commandResultDto;

            if (!CommandToResultDataType.TryGetValue(commandResultDto.Command.ToLower(), out var dataType))
                return commandResultDto;

            commandResultDto.Data = dataNode.Deserialize(dataType, options) as CommandResultDataDto;
            return commandResultDto;
        }
        catch
        {
            return null;
        }
    }

    private (CommandResultDto? CommandResultDto, JsonNode? DataNode) ReadWithoutData(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var namingPolicy = options.PropertyNamingPolicy;

        var optionsWithoutThisConverter = new JsonSerializerOptions(options);
        optionsWithoutThisConverter.Converters.Remove(this);

        if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
            return (null, null);

        const string dataName = nameof(CommandResultDto.Data);
        var dataPropertyName = GetCommandResultDtoPropertyName(dataName, namingPolicy);

        var dataNode = jsonObject[dataPropertyName];
        jsonObject.Remove(dataPropertyName);

        var commandResultDto = jsonObject.Deserialize<CommandResultDto>(optionsWithoutThisConverter);
        return (commandResultDto, dataNode);
    }

    public override void Write(Utf8JsonWriter writer, CommandResultDto? value, JsonSerializerOptions options)
    {
        try
        {
            var namingPolicy = options.PropertyNamingPolicy;

            var jsonObject = WriteWithoutData(value, options);
            if (jsonObject == null)
                return;

            const string dataName = nameof(CommandResultDto.Data);
            var dataPropertyName = GetCommandResultDtoPropertyName(dataName, namingPolicy);

            if (jsonObject[dataPropertyName] != null && value?.Data != null)
                jsonObject[dataPropertyName] = JsonSerializer.SerializeToNode(value.Data, value.Data.GetType(), options);

            jsonObject.WriteTo(writer);
        }
        catch
        {
            // ignored
        }
    }

    private JsonNode? WriteWithoutData(CommandResultDto? value, JsonSerializerOptions options)
    {
        var optionsWithoutConverter = new JsonSerializerOptions(options);
        optionsWithoutConverter.Converters.Remove(this);

        return JsonSerializer.SerializeToNode(value, optionsWithoutConverter);
    }
}