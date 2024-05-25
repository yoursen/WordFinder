using SQLite;

namespace WordFinder.Models;

[Table("GameWordCategories")]
public class GameWordCategory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public override string ToString() => Name ?? base.ToString();
}
