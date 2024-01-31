using Cadmus.Graph.Api.Models;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cadmus.Graph.Api.Controllers;

//[Route("api/[controller]")]
[ApiController]
public sealed class GraphController : ControllerBase
{
    private readonly IGraphRepository _repository;

    public GraphController(IGraphRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("api/graph/nodes")]
    public DataPage<UriNode> GetNodes([FromQuery]
        NodeFilterBindingModel model)
    {
        return _repository.GetNodes(model.ToNodeFilter());
    }

    [HttpGet("api/graph/nodes/{id}", Name = "GetNode")]
    public ActionResult GetNode([FromRoute] int id)
    {
        UriNode? node = _repository.GetNode(id);
        if (node == null) return NotFound();
        return Ok(node);
    }

    [HttpGet("api/graph/nodes-set")]
    public IList<UriNode?> GetNodeSet([FromQuery] IList<int> ids)
    {
        return _repository.GetNodes(ids);
    }

    [HttpGet("api/graph/nodes-by-uri")]
    public ActionResult GetNodeByUri([FromQuery] string uri)
    {
        UriNode? node = _repository.GetNodeByUri(uri);
        if (node == null) return NotFound();
        return Ok(node);
    }

    [HttpGet("api/graph/walk/triples")]
    public DataPage<TripleGroup> GetTripleGroups([FromQuery]
        TripleFilterBindingModel model)
    {
        return _repository.GetTripleGroups(
            model.ToTripleFilter(), model.Sort ?? "Cu");
    }

    [HttpGet("api/graph/walk/nodes")]
    public DataPage<UriNode> GetLinkedNodes([FromQuery]
        LinkedNodeFilterBindingModel model)
    {
        return _repository.GetLinkedNodes(model.ToLinkedNodeFilter());
    }

    [HttpGet("api/graph/walk/nodes/literal")]
    public DataPage<UriTriple> GetLinkedLiterals([FromQuery]
        LinkedLiteralFilterBindingModel model)
    {
        return _repository.GetLinkedLiterals(model.ToLinkedLiteralFilter());
    }
}
