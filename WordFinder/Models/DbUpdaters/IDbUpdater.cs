using SQLite;

namespace WordFinder.Models.DbUpdaters;

public interface IDbUpdater
{
    Task Update(SQLiteAsyncConnection destinationDb, SQLiteAsyncConnection sourceDb);
}
