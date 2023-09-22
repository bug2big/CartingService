using Carting.Data.Common;
using Carting.Data.Entities;

namespace Carting.Data.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
{
    private readonly ILiteDatabaseAsync _context;
    protected readonly ILiteCollectionAsync<TEntity> Collection;

    protected BaseRepository(ILiteDbContextProvider liteDbContextProvider)
    {
        _context = liteDbContextProvider.Context;
        Collection = liteDbContextProvider.Context.GetCollection<TEntity>();
    }

    public async Task DeleteAsync(Guid entityId)
    {
        await Collection.DeleteAsync(new BsonValue(entityId));
        await _context.CommitAsync();
    }

    public async Task UpsertAsync(TEntity entity)
    {
        await Collection.UpsertAsync(entity);
        await _context.CommitAsync();
    }
}
