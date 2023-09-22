namespace Carting.Models;

public class CartModel
{
    public Guid CartId { get; set; }

    public IList<CartItemModel> Items { get; set; } = null!;
}
