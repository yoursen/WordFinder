using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder;

public partial class GameModel : ObservableObject
{
    private const int GameLettersCount = 25;
    private WordsDatabase _db;
    public GameModel(WordsDatabase db)
    {
        _db = db;
        CreateGameField();
    }

    [ObservableProperty] private GameWord _guessWord;
    [ObservableProperty] private string _userWord;
    [ObservableProperty] private GameLetter[] _Letters;

    public async Task Next()
    {
        GuessWord = await _db.GetRandomWord();
        UserWord = new string('_', GuessWord.Word.Length);
        CreateGameField();
    }

    public async Task Reset()
    {
        Letters = Array.Empty<GameLetter>();
        GuessWord = GameWord.Empty;
        await Task.CompletedTask;
    }

    public void ToggleButton(CharButtonView ch)
    {
        if (UserWord.Contains("_"))
            UserWord = string.Empty;

        UserWord += ch.Title;
    }

    private void CreateGameField()
    {
        var letters = new GameLetter[GameLettersCount];
        if (GuessWord is not null)
        {
            foreach (var ch in GuessWord.Word)
            {
                var cellIndex = Random.Shared.Next(0, GameLettersCount - 1);
                if (letters[cellIndex] is not null)
                    continue;

                letters[cellIndex] = new GameLetter(ch.ToString().ToUpper());
            }
        }

        for (int i = 0; i < GameLettersCount; i++)
        {
            if (letters[i] is not null)
                continue;

            var letterStr = ((char)Random.Shared.Next(65, 90)).ToString();
            //var gameLetter = new GameLetter(letterStr);
            var gameLetter = new GameLetter("");
            letters[i] = gameLetter;
        }
        Letters = letters;
    }
}