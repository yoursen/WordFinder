using SQLite;
using WordFinder.Services;

namespace WordFinder.Models;

public class GameDatabase
{
    private SQLiteAsyncConnection _database;
    private readonly LicenseService _license;
    private readonly GameSettings _gameSettings;

    public List<GameWordCategory> Categories { get; private set; }

    public GameDatabase(LicenseService license, GameSettings gameSettings)
    {
        _license = license;
        _gameSettings = gameSettings;
    }

    private string TableSuffix => _gameSettings.Language switch
    {
        GameLanguage.Ukrainian => "_uk_UA",
        _ => string.Empty,
    };

    private GameLanguage? Language { get; set; }

    public async Task Init()
    {
        if (_database is not null)
        {
            if (Language != null && Language.Value != _gameSettings.Language)
            {
                Language = _gameSettings.Language;
                await InitCategories();
            }
            return;
        }

        Language = _gameSettings.Language;
        await DeployDB();

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await CreateScoreDB("GameScore");
        await CreateScoreDB("GameScore_uk_UA");

        await InitCategories();
    }

    private async Task CreateScoreDB(string tableName)
    {
        var res = await _database.ExecuteAsync(@$"CREATE TABLE IF NOT EXISTS {tableName} (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Score INTEGER,
                                        GameDuration INTEGER)");
    }

    private async Task InitCategories()
    {
        Categories = await _database.QueryAsync<GameWordCategory>($"SELECT * FROM GameWordCategories{TableSuffix}");
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
        string RandomWordQuery = $"SELECT * FROM GameWords{TableSuffix} WHERE Id = (SELECT Id FROM GameWords{TableSuffix} WHERE {licenseFilter} IsPlayed = FALSE ORDER BY RANDOM() LIMIT 1)";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        var word = randomWord.FirstOrDefault();

        if (word is not null)
            SetWordCategory(word);

        return word;
    }

    public async Task<GameWord[]> GameRandomWords(int count)
    {
        await Init();

        string RandomWordQuery = $"SELECT * FROM GameWords{TableSuffix} ORDER BY RANDOM() LIMIT {count}";
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

        string sql = $"UPDATE GameWords{TableSuffix} SET IsPlayed = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isPlayed, id);
    }

    public async Task<int> ResetIsPlayed()
    {
        await Init();

        string sql = $"UPDATE GameWords{TableSuffix} SET IsPlayed = FALSE WHERE IsPlayed = TRUE AND IsAnswered = FALSE";
        return await _database.ExecuteAsync(sql);
    }

    public async Task SetIsAnswered(int id, bool isAnswered)
    {
        await Init();

        string sql = $"UPDATE GameWords{TableSuffix} SET IsAnswered = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isAnswered, id);
    }

    public async Task<int> CountWordsAnswered()
    {
        string predicate;
        if (_license.IsFree)
            predicate = "IsAnswered = TRUE AND IsPro = FALSE";
        else
            predicate = "IsAnswered = TRUE";

        return await CountWords(predicate);
    }
    public async Task<int> CountWordsNotAnswered()
    {
        string predicate;
        if (_license.IsFree)
            predicate = "IsAnswered = FALSE AND IsPro = FALSE";
        else
            predicate = "IsAnswered = FALSE";

        return await CountWords(predicate);
    }
    public async Task<int> CountWordsPro() => await CountWords("IsPro = TRUE");
    public async Task<int> CountWords()
    {
        string predicate;
        if (_license.IsFree)
            predicate = "IsPro = FALSE";
        else
            predicate = "1 = 1";

        return await CountWords(predicate);
    }

    private async Task<int> CountWords(string where)
    {
        var words = await _database.QueryAsync<GameWord>($"SELECT * FROM GameWords{TableSuffix} WHERE {where}");
        return words.Count();
    }

    public async Task ResetIsAnswered()
    {
        await Init();

        string sql = $"UPDATE GameWords{TableSuffix} SET IsAnswered = FALSE";
        await _database.ExecuteAsync(sql);
    }

    public async Task<GameScore> GetLastGameScore()
    {
        await Init();

        string query = $"SELECT * FROM GameScore{TableSuffix} WHERE ID = (SELECT MAX(ID) FROM GameScore{TableSuffix});";
        var result = await _database.QueryAsync<GameScore>(query);
        return result.FirstOrDefault();
    }

    public async Task AddGameScore(GameScore gameScore)
    {
        await Init();
        await _database.ExecuteAsync(@$"INSERT INTO GameScore{TableSuffix} (Score, GameDuration) VALUES (?, ?)",
                                  gameScore.Score, gameScore.GameDuration);
    }

    public async Task<GameScore> GetBestGameScore(int gameDuration)
    {
        await Init();
        var query = $"SELECT * FROM GameScore{TableSuffix} WHERE Score = (SELECT MAX(Score) FROM GameScore{TableSuffix} WHERE GameDuration = ?) LIMIT 1;";
        var result = await _database.QueryAsync<GameScore>(query, gameDuration);
        return result.FirstOrDefault();
    }

}