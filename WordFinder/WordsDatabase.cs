using SQLite;

public class WordsDatabase
{
    private SQLiteAsyncConnection _database;

    public WordsDatabase()
    {
    }

    async Task Init()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await _database.CreateTableAsync<CountItem>();
    }

    public async Task<List<CountItem>> GetItemsAsync()
    {
        await Init();
        return await _database!.Table<CountItem>().Take(30).ToListAsync();
    }

    public async Task<CountItem> GetLast()
    {
        await Init();
        return await _database!.Table<CountItem>().OrderByDescending(i => i.DateTime).FirstOrDefaultAsync();
    }

    public async Task AddItem(CountItem countItem)
    {
        await _database!.InsertAsync(countItem);
    }
}