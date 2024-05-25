using SQLite;

namespace WordFinder.Models;

[Table("GameWords")]
public class GameWord
{
    public static GameWord Empty = new GameWord(string.Empty, string.Empty);
    public GameWord() { }
    public GameWord(string word, string description)
    {
        Word = word;
        Description = description;
    }

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Word { get; set; }
    public string Description { get; set; }
    public bool IsPlayed { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsPro { get; set; }
    public int CategoryId { get; set; }
    public ComplexityEnum Complexity { get; set; }

    [Ignore]
    public GameWordCategory Category { get; set; }
}
