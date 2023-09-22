using System.ComponentModel.DataAnnotations;

namespace Carting.Data.Entities;

public record Cart : BaseEntity
{
    public Guid CartId { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Quantity { get; set; }
}

