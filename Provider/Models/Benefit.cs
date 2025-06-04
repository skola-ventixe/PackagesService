using System.ComponentModel.DataAnnotations.Schema;

namespace Provider.Models;

public class  Benefit
{
    public int? Id { get; set; }

    [ForeignKey(nameof(Package))]
    public string PackageId { get; set; } = null!;

    public string Description { get; set; } = null!;
}
