using Cadmus.Core;
using System;
using System.Text.Json;

namespace Cadmus.TestBase;

internal static class TestHelper
{
    private static readonly JsonSerializerOptions _options =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static string SerializePart(IPart part)
    {
        if (part == null)
            throw new ArgumentNullException(nameof(part));

        return JsonSerializer.Serialize(part, part.GetType(), _options);
    }

    public static T? DeserializePart<T>(string json)
        where T : class, IPart, new()
    {
        if (json == null)
            throw new ArgumentNullException(nameof(json));

        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
