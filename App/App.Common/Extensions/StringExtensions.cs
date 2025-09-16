namespace App.Common.Extensions;

public static class StringExtensions
{
    public static string EmptyIfNull(this string? str)
    {
        return str ?? string.Empty;
    }
}