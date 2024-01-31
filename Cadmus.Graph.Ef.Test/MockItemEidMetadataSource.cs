using Cadmus.Core.Storage;
using Cadmus.Graph.Adapters;
using System.Collections.Generic;

namespace Cadmus.Graph.Ef.Test;

internal sealed class MockItemEidMetadataSource : IMetadataSource
{
    private string? _pid;
    private string? _eid;

    public MockItemEidMetadataSource(string? pid, string? eid)
    {
        _pid = pid;
        _eid = eid;
    }

    public void Set(string? pid, string? eid)
    {
        _pid = pid;
        _eid = eid;
    }

    public void Supply(GraphSource source, IDictionary<string, object> metadata,
        ICadmusRepository? repository, object? context = null)
    {
        if (_pid != null) metadata["metadata-pid"] = _pid;
        if (_eid != null) metadata["item-eid"] = _eid;
    }
}
