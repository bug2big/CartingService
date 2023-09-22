namespace Carting.Data.Common;

public interface ILiteDbContextProvider
{
    public LiteDatabaseAsync Context { get; init; }
}

