namespace WordFinder;

public class GamePageViewModel : BindableObject
{
    private WordsDatabase _db;
    public GamePageViewModel(WordsDatabase db)
    {
        _db = db;
    }

    public static readonly BindableProperty LettersProperty =
            BindableProperty.Create(nameof(Letters), typeof(GameLetter[]), typeof(GamePageViewModel), default(GameLetter[]));

    public GameLetter[] Letters
    {
        get => (GameLetter[])GetValue(LettersProperty);
        set => SetValue(LettersProperty, value);
    }

    public static readonly BindableProperty GuessWordProperty =
            BindableProperty.Create(nameof(GuessWord), typeof(string), typeof(GamePageViewModel), "");

    public string GuessWord
    {
        get => (string)GetValue(GuessWordProperty);
        set => SetValue(GuessWordProperty, value);
    }

    public static readonly BindableProperty UserWordProperty =
            BindableProperty.Create(nameof(UserWord), typeof(string), typeof(GamePageViewModel), "");

    public string UserWord
    {
        get => (string)GetValue(UserWordProperty);
        set => SetValue(UserWordProperty, value);
    }

    private void RefreshGameField() => Letters = GreateGameField();

    private GameLetter[] GreateGameField()
    {
        var field = new List<GameLetter>();
        for (int i = 0; i < 25; i++)
        {
            var letterStr = ((char)Random.Shared.Next(65, 90)).ToString();
            var gameLetter = new GameLetter(letterStr);
            field.Add(gameLetter);
        }
        return field.ToArray();
    }

    public async Task Next()
    {
        var gameWord = await _db.GetRandomWord();
        GuessWord = gameWord.Description;
        UserWord = new string('_', gameWord.Word.Length);

        RefreshGameField();
    }

    public void Reset()
    {
        Letters = Array.Empty<GameLetter>();
        GuessWord = string.Empty;
        UserWord = string.Empty;
    }

    public void ToggleButton(CharButtonView ch)
    {
        if (UserWord.Contains("_"))
            UserWord = string.Empty;

        UserWord += ch.Title;
    }
}