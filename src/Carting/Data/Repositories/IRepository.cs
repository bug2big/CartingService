namespace Carting.Data.Repositories;

public interface IRepository<TEntity>
{
    Task DeleteAsync(Guid entityId);

    Task UpsertAsync(TEntity entity);
}

