using Carting.Data.Entities;

namespace Carting.Data.Common;

public class DbMapper : BsonMapper
{
    public DbMapper()
    {
        Global.Entity<BaseEntity>()
            .Id(x => x.Id);
    }
}
