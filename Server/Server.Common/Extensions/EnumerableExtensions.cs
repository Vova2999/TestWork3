using System.Diagnostics.CodeAnalysis;

namespace Server.Common.Extensions;

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<TValue>([NotNullWhen(false)] this IEnumerable<TValue>? values)
    {
        return values == null || !values.Any();
    }

    public static bool IsSignificant<TValue>([NotNullWhen(true)] this IEnumerable<TValue>? values)
    {
        return values != null && values.Any();
    }
}