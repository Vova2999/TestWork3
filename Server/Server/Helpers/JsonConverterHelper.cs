using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Helpers;

public static class JsonConverterHelper
{
    public static string GetPropertyName(Type type, string propertyName, JsonNamingPolicy? namingPolicy)
    {
        var propertyInfo = type.GetProperty(propertyName);
        return GetPropertyName(propertyInfo, propertyName, namingPolicy);
    }

    private static string GetPropertyName(MemberInfo? memberInfo, string propertyName, JsonNamingPolicy? namingPolicy)
    {
        var jsonPropertyNameAttr = memberInfo?.GetCustomAttribute<JsonPropertyNameAttribute>();
        if (jsonPropertyNameAttr != null)
            return jsonPropertyNameAttr.Name;

        var memberInfoName = memberInfo?.Name ?? propertyName;
        return namingPolicy?.ConvertName(memberInfoName) ?? memberInfoName;
    }
}