using SQLite;

namespace WordFinder.Models.DbUpdaters;

public class DbUpdaterUALanguage : IDbUpdater
{
    public async Task Update(SQLiteAsyncConnection destinationDb, SQLiteAsyncConnection sourceDb)
    {
        var res = await destinationDb.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS GameWordCategories_uk_UA (
                Id INTEGER NOT NULL CONSTRAINT PK_GameWordCategories_uk_UA PRIMARY KEY AUTOINCREMENT,
                Name TEXT NULL
            );");

        res = await destinationDb.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS GameWords_uk_UA (
                Id INTEGER NOT NULL CONSTRAINT PK_GameWords_uk_UA PRIMARY KEY AUTOINCREMENT,
                Word TEXT NULL,
                Description TEXT NULL,
                IsPlayed INTEGER NOT NULL,
                IsAnswered INTEGER NOT NULL,
                IsPro INTEGER NOT NULL,
                CategoryId INTEGER NULL,
                Complexity INTEGER NOT NULL,
                CONSTRAINT FK_GameWords_uk_UA_GameWordCategories_uk_UA_CategoryId FOREIGN KEY (CategoryId) REFERENCES GameWordCategories_uk_UA (Id)
            );");

        res = await destinationDb.ExecuteAsync(@"
            CREATE INDEX IF NOT EXISTS IX_GameWords_uk_UA_CategoryId
            ON GameWords_uk_UA (CategoryId);");

        var categories = await sourceDb.QueryAsync<GameWordCategory>($"SELECT * FROM GameWordCategories_uk_UA");
        foreach (var category in categories)
        {
            var insertQueryCategory = @"
                INSERT INTO GameWordCategories_uk_UA (Name)
                VALUES (?)";

            res = await destinationDb.ExecuteAsync(insertQueryCategory, category.Name);
        }

        var words = await sourceDb.QueryAsync<GameWord>($"SELECT * FROM GameWords_uk_UA");
        foreach (var gameWord in words)
        {
            var insertQueryWord = @"
                INSERT INTO GameWords_uk_UA (Word, Description, IsPlayed, IsAnswered, IsPro, CategoryId, Complexity)
                VALUES (?, ?, ?, ?, ?, ?, ?);";

            res = await destinationDb.ExecuteAsync(insertQueryWord,
                gameWord.Word, gameWord.Description, gameWord.IsPlayed,
                gameWord.IsAnswered, gameWord.IsPro, gameWord.CategoryId,
                gameWord.Complexity);
        }
    }
}
