using SQLite;

namespace WordFinder.Models.DbUpdaters;

public class DbUpdater
{
    private readonly SQLiteAsyncConnection _destinationDb;
    private SQLiteAsyncConnection _sourceDb;
    private readonly List<IDbUpdater> _updaters;

    public DbUpdater(SQLiteAsyncConnection destinationDb)
    {
        _destinationDb = destinationDb;
        _updaters = new List<IDbUpdater>()
        {
            new DbUpdaterUALanguage()
        };
    }

    public async Task Update()
    {
        var currentVersion = await GetCurrentVersion();
        if (currentVersion == _updaters.Count)
            return;

        await ResourceManager.DeployDB(Constants.DatabaseUpdateFilename, true);
        _sourceDb = new SQLiteAsyncConnection(Constants.DatabaseUpdatePath, Constants.Flags);

        for (int i = currentVersion; i < _updaters.Count; i++)
            await _updaters[i].Update(_destinationDb, _sourceDb);

        await SetCurrentVersion(_updaters.Count);

        await _sourceDb.CloseAsync();
    }

    private Task<int> GetCurrentVersion()
        => _destinationDb.ExecuteScalarAsync<int>("PRAGMA user_version;");

    private Task SetCurrentVersion(int version)
        => _destinationDb.ExecuteAsync($"PRAGMA user_version={version};");
}