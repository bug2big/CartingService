using Carting.Settings;

namespace Carting.Data.Common;

public class LiteDbContextProvider : ILiteDbContextProvider
{
    public LiteDatabaseAsync Context { get; init; }

    public LiteDbContextProvider(
        IOptions<LiteDbSettings> liteDbSettingOptions)
    {
        try
        {
            var dbMapper = new DbMapper();
            var db = new LiteDatabaseAsync(liteDbSettingOptions.Value.DatabasePath, dbMapper);
            Context = db!;
        }
        catch (Exception ex)
        {
            throw new Exception("Can find or create LiteDb database.", ex);
        }
    }
}