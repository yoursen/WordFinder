public static class Constants
{
    public const string DatabaseFilename = "Words.db";
    public const string DatabaseUpdateFilename = "WordsUpdate.db";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    public static string DatabaseUpdatePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseUpdateFilename);
}