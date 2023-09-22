namespace Carting.Models;

public class CartItemModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Image { get; init; }

    public double Price { get; init; }

    public int Quantity { get; init; }
}

