using SQLite;

namespace WordFinder.Models;

public class GameScore
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int Score { get; set; }
    public int GameDuration { get; set; }
}