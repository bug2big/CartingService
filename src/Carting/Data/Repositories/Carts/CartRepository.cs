using Carting.Data.Common;
using Carting.Data.Entities;

namespace Carting.Data.Repositories.Carts;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(ILiteDbContextProvider liteDbContextProvider)
        : base(liteDbContextProvider)
    {
    }

    public Task<IEnumerable<Cart>> GetCartItemsAsync(Guid cartItemId) 
    {
        return Collection
            .Query()
            .Where(c => c.Id == cartItemId)
            .ToEnumerableAsync();
    }

    public Task<IEnumerable<Cart>> GetItemsByCartIdAsync(Guid cartId)
    {
        return Collection
            .Query()
            .Where(c => c.CartId == cartId).ToEnumerableAsync();
    }

    public async Task DeleteCartItemByIdAndCartIdAsync(Guid cartId, Guid cartItemId)
    {
        var cart = await Collection.Query()
            .Where(c => c.Id == cartItemId && c.CartId == cartId)
            .FirstOrDefaultAsync();
        
        if (cart != null) 
        {
            await DeleteAsync(cart.Id);
        }
    }
}