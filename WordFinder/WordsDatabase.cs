using SQLite;
using WordFinder;

public class WordsDatabase
{
    private SQLiteAsyncConnection _database;

    public async Task Init()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        if (await _database.CreateTableAsync<GameWord>() == CreateTableResult.Created)
        {
            await _database.InsertAllAsync(
                new[]
                {
                    new GameWord("apple","A fruit typically round in shape, with various colors like red, green, or yellow. It's commonly eaten raw, used in cooking, or for making beverages like cider."),
                    new GameWord("banana","A curved fruit with a yellow skin when ripe, rich in potassium and often eaten raw or used in desserts and smoothies."),
                    new GameWord("pizza","A savory dish consisting of a round, flattened base of dough topped with tomato sauce, cheese, and various toppings, baked in an oven."),
                    new GameWord("bread","A staple food made from flour, water, and yeast, typically baked and often sliced for sandwiches or served with meals."),
                    new GameWord("chocolate","A sweet, usually brown, food preparation of roasted and ground cacao seeds that's typically sweetened and often used in desserts or eaten as a snack."),
                    new GameWord("coffee","A brewed beverage prepared from roasted coffee beans, typically served hot and enjoyed for its stimulating effect."),
                    new GameWord("cake","A sweet baked dessert made from flour, sugar, eggs, and flavorings, often frosted and decorated for celebrations."),
                    new GameWord("soup","A liquid food made by boiling meat, vegetables, or other ingredients in stock or water, often served hot as a starter or main course."),
                    new GameWord("rice","A cereal grain that's a staple food for a large part of the world's population, typically served cooked as a side dish or base for other dishes."),
                    new GameWord("cheese","A food made from the curdled and compressed milk of various animals, typically savory in flavor and used in cooking or as a snack."),
                }
            );
        }
    }

    public async Task<GameWord> GetRandomWord()
    {
        await Init();

        const string RandomWordQuery = "SELECT * FROM GameWord WHERE Id = (SELECT Id FROM GameWord WHERE IsPlayed = FALSE ORDER BY RANDOM() LIMIT 1)";
        var randomWord = await _database.QueryAsync<GameWord>(RandomWordQuery);
        return randomWord.FirstOrDefault();
    }

    public async Task SetIsPlayed(int id, bool isPlayed)
    {
        await Init();

        const string sql = $"UPDATE GameWord SET IsPlayed = ? WHERE Id = ?";
        var res = await _database.ExecuteAsync(sql, isPlayed, id);
    }

    public async Task ResetIsPlayed()
    {
        await Init();

        const string sql = $"UPDATE GameWord SET IsPlayed = FALSE";
        var res = await _database.ExecuteAsync(sql);
    }
}