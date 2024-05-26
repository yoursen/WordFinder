namespace WordFinder.Models;

public static class ResourceManager
{
    public static async Task DeployDB(string databaseFileName, bool overwrite = false)
    {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, databaseFileName);
        if (File.Exists(targetFile))
        {
            if (overwrite)
                File.Delete(targetFile);
            else
                return;
        }

        using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(Constants.DatabaseFilename);
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
    }
}