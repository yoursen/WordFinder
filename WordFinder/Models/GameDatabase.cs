using SQLite;
using WordFinder.Services;

namespace WordFinder.Models;

public class GameDatabase
{
    private SQLiteAsyncConnection _database;
    private LicenseService _license;

    public GameWordCategory[] Categories { get; private set; }

    public GameDatabase(LicenseService license)
    {
        _license = license;
    }

    public async Task Init()
    {
        if (_database is not null)
            return;

        await DeployDB();

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await _database.CreateTableAsync<GameScore>();

        Categories = await _database.Table<GameWordCategory>().ToArrayAsync();
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

        string licenseFilter = _license.IsFree ? "IsPro = 0 AND" : string.Empty;
        string RandomWordQuery = $"SELECT * FROM GameWords WHERE Id = (SELECT Id FROM GameWords WHERE {licenseFilter} IsPlayed = FALSE ORDER BY RANDOM() LIMIT 1)";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        var word = randomWord.FirstOrDefault();

        if (word is not null)
            SetWordCategory(word);

        return word;
    }

    public async Task<GameWord[]> GameRandomWords(int count)
    {
        await Init();

        string RandomWordQuery = $"SELECT * FROM GameWords ORDER BY RANDOM() LIMIT {count}";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        return randomWord.Select(SetWordCategory).ToArray();
    }

    private GameWord SetWordCategory(GameWord word)
    {
        word.Category = Categories.FirstOrDefault(c => c.Id == word.CategoryId);
        return word;
    }

    public async Task SetIsPlayed(int id, bool isPlayed)
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsPlayed = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isPlayed, id);
    }

    public async Task<int> ResetIsPlayed()
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsPlayed = FALSE WHERE IsPlayed = TRUE AND IsAnswered = FALSE";
        return await _database.ExecuteAsync(sql);
    }

    public async Task SetIsAnswered(int id, bool isAnswered)
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsAnswered = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isAnswered, id);
    }

    public async Task<int> CountWordsAnswered()
    {
        System.Linq.Expressions.Expression<Func<GameWord, bool>> predicate;
        if (_license.IsFree)
            predicate = w => w.IsAnswered && !w.IsPro;
        else
            predicate = w => w.IsAnswered;

        return await CountWords(predicate);
    }
    public async Task<int> CountWordsNotAnswered()
    {
        System.Linq.Expressions.Expression<Func<GameWord, bool>> predicate;
        if (_license.IsFree)
            predicate = w => !w.IsAnswered && !w.IsPro;
        else
            predicate = w => !w.IsAnswered;

        return await CountWords(predicate);
    }
    public async Task<int> CountWordsPro() => await CountWords(w => w.IsPro);
    public async Task<int> CountWords()
    {
        System.Linq.Expressions.Expression<Func<GameWord, bool>> predicate;
        if (_license.IsFree)
            predicate = w => !w.IsPro;
        else
            predicate = w => true;

        return await CountWords(predicate);
    }

    private async Task<int> CountWords(System.Linq.Expressions.Expression<Func<GameWord, bool>> predicate)
        => await _database.Table<GameWord>().Where(predicate).CountAsync();

    public async Task ResetIsAnswered()
    {
        await Init();

        const string sql = $"UPDATE GameWords SET IsAnswered = FALSE";
        await _database.ExecuteAsync(sql);
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