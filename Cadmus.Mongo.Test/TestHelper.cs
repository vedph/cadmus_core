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
        ArgumentNullException.ThrowIfNull(part);

        return JsonSerializer.Serialize(part, part.GetType(), _options);
    }

    public static T? DeserializePart<T>(string json)
        where T : class, IPart, new()
    {
        ArgumentNullException.ThrowIfNull(json);

        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
