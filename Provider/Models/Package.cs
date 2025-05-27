using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provider.Models;

public class Package
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string EventId { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? Description { get; set; } = null!;

    public bool Seated { get; set; }
    public string Placement { get; set; } = null!;

    public ICollection<Benefit>? Benefits { get; set; } = [];

}
