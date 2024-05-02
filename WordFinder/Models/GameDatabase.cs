using SQLite;

namespace WordFinder.Models;

public class GameDatabase
{
    private SQLiteAsyncConnection _database;

    public async Task Init()
    {
        if (_database is not null)
            return;

        await DeployDB();

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await _database.CreateTableAsync<GameScore>();
    }

    public async Task DeployDB()
    {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, Constants.DatabaseFilename);
        if (File.Exists(targetFile))
        {
            return;
        }

        using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(Constants.DatabaseFilename);
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
    }

    public async Task<GameWord> GameRandomGameWord()
    {
        await Init();

        const string RandomWordQuery = "SELECT * FROM GameWords WHERE Id = (SELECT Id FROM GameWords WHERE IsPlayed = FALSE ORDER BY RANDOM() LIMIT 1)";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        return randomWord.FirstOrDefault();
    }

    public async Task<GameWord[]> GameRandomWords(int count)
    {
        await Init();

        string RandomWordQuery = $"SELECT * FROM GameWords ORDER BY RANDOM() LIMIT {count}";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        return randomWord.ToArray();
    }

    public async Task SetIsPlayed(int id, bool isPlayed)
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsPlayed = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isPlayed, id);
    }

    public async Task ResetIsPlayed()
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsPlayed = FALSE";
        var res = await _database.ExecuteAsync(sql);
    }

    public async Task<GameScore> GetLastGameScore()
    {
        await Init();

        const string query = "SELECT * FROM GameScore WHERE ID = (SELECT MAX(ID) FROM GameScore);";
        var result = await _database.QueryAsync<GameScore>(query);
        return result.FirstOrDefault();
    }

    public async Task AddGameScore(GameScore gameScore)
    {
        await Init();
        await _database.InsertAsync(gameScore);
    }

    public async Task<GameScore> GetBestGameScore(int gameDuration)
    {
        await Init();
        var query = "SELECT * FROM GameScore WHERE Score = (SELECT MAX(Score) FROM GameScore WHERE GameDuration = ?) LIMIT 1;";
        var result = await _database.QueryAsync<GameScore>(query, gameDuration);
        return result.FirstOrDefault();
    }

}