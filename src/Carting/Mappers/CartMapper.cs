using Carting.Data.Entities;
using Carting.Models;

namespace Carting.Mappers;

[Mapper]

public partial class CartMapper
{
    public partial CartItemModel MapToCartModel(Cart cart);

    public Cart MapToCart(CartItemModel cartItemModel, Guid cartId) 
    {
        var dto = MapToCart(cartItemModel);
        dto.CartId = cartId;
        return dto;
    }


    private partial Cart MapToCart(CartItemModel cartItemModel);
}
