using System.ComponentModel.DataAnnotations;

namespace Carting.Data.Entities;

public record BaseEntity
{
    [Required]
    public Guid Id { get; init; }
}

