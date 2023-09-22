using Carting.Data.Repositories.Carts;
using Carting.Mappers;
using Carting.Models;

namespace Carting.Services.Carts;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly CartMapper _cartMapper;

    public CartService(
        ICartRepository cartRepository,
        CartMapper cartMapper)
    {
        _cartRepository = cartRepository;
        _cartMapper = cartMapper;
    }

    public async Task<IList<CartItemModel>> GetCartItemsByCartIdAsync(Guid cartId)
    {
        var cartItems = await _cartRepository.GetItemsByCartIdAsync(cartId);

        return cartItems.Select(_cartMapper.MapToCartModel).ToList();
    }

    public async Task UpsertAsync(CartItemModel cartItemModel, Guid cartId)
    {
        var cart = _cartMapper.MapToCart(cartItemModel, cartId);
        await _cartRepository.UpsertAsync(cart);
    }

    public async Task DeleteAsync(Guid cartId, Guid cartItemId)
    {
        await _cartRepository.DeleteCartItemByIdAndCartIdAsync(cartId, cartItemId);
    }
}

