using SQLite;

namespace WordFinder;

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

    // further development 
    // public string Category { get; set; }
    // public ComplexityEnum Complexity { get; set; }
    // public int PlayTime { get; set; }
    // public bool IsSuccess { get; set; }
}