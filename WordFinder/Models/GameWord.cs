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
    public int CategoryId { get; set; }
    public ComplexityEnum Complexity { get; set; }

    [Ignore]
    public GameWordCategory Category { get; set; }
}

[Table("GameWordCategories")]
public class GameWordCategory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public override string ToString() => Name ?? base.ToString();
}

public enum ComplexityEnum
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}