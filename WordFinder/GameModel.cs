using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder;

public partial class GameModel : ObservableObject
{
    private const int GridSize = 5;
    private WordsDatabase _db;
    private WordFitter _wordFitter;
    public GameModel(WordsDatabase db, WordFitter wordFitter)
    {
        _db = db;
        _wordFitter = wordFitter;
        CreateGameField();
    }

    [ObservableProperty] private GameWord _guessWord;
    [ObservableProperty] private string _userWord;
    [ObservableProperty] private GameLetter[] _Letters;

    public async Task Next()
    {
        bool success;
        do
        {
            GuessWord = await _db.GetRandomWord();
            UserWord = new string('_', GuessWord.Word.Length);
            success = CreateGameField();
        } while (!success);
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

    private bool CreateGameField()
    {
        _wordFitter.Initialize(GridSize);
        if (GuessWord is not null)
        {
            if (!_wordFitter.FitWord(GuessWord))
            {
                return false;
            }
        }
        _wordFitter.FitBlank();
        Letters = _wordFitter.Flush();
        return true;
    }
}