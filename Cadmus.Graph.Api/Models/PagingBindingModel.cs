using System.ComponentModel.DataAnnotations;

namespace Cadmus.Graph.Api.Models;

public class PagingBindingModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; }

    [Required]
    [Range(1, 50)]
    public int PageSize { get; set; }
}
