using System.Diagnostics.CodeAnalysis;

namespace Server.Common.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string? NullIfEmpty(this string str)
    {
        return string.IsNullOrEmpty(str) ? null : str;
    }

    public static string EmptyIfNull(this string? str)
    {
        return str ?? string.Empty;
    }
}