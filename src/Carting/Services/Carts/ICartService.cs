using Carting.Models;

namespace Carting.Services.Carts;

public interface ICartService
{
    Task<IList<CartItemModel>> GetCartItemsByCartIdAsync(Guid cartId);

    Task UpsertAsync(CartItemModel cartItemModel, Guid cartId);

    Task DeleteAsync(Guid cartId, Guid cartItemId);
}
