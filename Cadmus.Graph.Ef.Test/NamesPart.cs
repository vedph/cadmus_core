using Cadmus.Core;
using Fusi.Tools.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Graph.Ef.Test;

/// <summary>
/// Mock part.
/// </summary>
/// <seealso cref="PartBase" />
[Tag("it.vedph.bricks.names")]
internal sealed class NamesPart : PartBase
{
    public IList<string> Names { get; set; }

    public NamesPart()
    {
        Names = new List<string>();
    }

    public override IList<DataPinDefinition> GetDataPinDefinitions()
    {
        return new[]
        {
            new DataPinDefinition
            {
                Name = "name",
                Type = DataPinValueType.String,
                Tip = "The name"
            }
        };
    }

    public override IEnumerable<DataPin> GetDataPins(IItem? item = null)
    {
        return Names?.Select(n => new DataPin
        {
            Name = "name",
            Value = n
        }) ?? Array.Empty<DataPin>();
    }
}
