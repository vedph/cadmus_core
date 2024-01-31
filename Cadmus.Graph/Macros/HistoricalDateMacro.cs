using Cadmus.Graph.Adapters;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Configuration;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace Cadmus.Graph.Macros;

/// <summary>
/// Historical date macro. This parses a <see cref="HistoricalDate"/> from
/// the received context, and returns either its sort value or its textual
/// representation.
/// <para>Tag: node-mapping-macro.historical-date</para>.
/// </summary>
[Tag("node-mapping-macro.historical-date")]
public sealed class HistoricalDateMacro : INodeMappingMacro
{
    private static readonly JsonSerializerOptions _options =
        new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        };

    private static HistoricalDate? ParseDate(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<HistoricalDate>(json, _options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return null;
        }
    }

    /// <summary>
    /// Run the macro function.
    /// </summary>
    /// <param name="context">The data context of the macro function.</param>
    /// <param name="args">The arguments: 0*=JSON representing the date,
    /// 1=the property of the date to return: <c>value</c> (default) or
    /// <c>text</c>.</param>
    /// <returns>Result or null.</returns>
    /// <exception cref="ArgumentNullException">template</exception>
    public string? Run(GraphSource? context, string[]? args)
    {
        if (args == null || args.Length == 0) return null;

        HistoricalDate? date = ParseDate(args[0] ?? "{}");
        if (date is null) return null;

        if (args.Length > 1 && args[1] == "text")
        {
            return date?.ToString(CultureInfo.InvariantCulture) ?? "";
        }

        return date?.GetSortValue()
            .ToString(CultureInfo.InvariantCulture) ?? "0";
    }
}
