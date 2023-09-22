using Carting.Data.Entities;

namespace Carting.Data.Repositories.Carts;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetCartItemsAsync(Guid cartItemId);

    Task<IEnumerable<Cart>> GetItemsByCartIdAsync(Guid cartId);

    Task DeleteCartItemByIdAndCartIdAsync(Guid cartId, Guid cartItemId);
}