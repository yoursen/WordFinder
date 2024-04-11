using System.Text;
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

    private List<GameLetter> _userWordLetters = new List<GameLetter>();

    [ObservableProperty] private GameWord _guessWord;
    [ObservableProperty] private string _userWord;
    [ObservableProperty] private GameLetter[] _Letters;

    public async Task Next()
    {
        bool success;
        do
        {
            GuessWord = await _db.GetRandomWord();
            _userWordLetters.Clear();
            UpdateUserWord();
            success = CreateGameField();
        } while (!success);
    }

    public async Task Hint()
    {
        var hintLetter = Letters.Where(el => el.IsMainLetter && !el.IsChecked)
               .OrderBy(el => el.LetterIndex)
               .FirstOrDefault();
        if (hintLetter is not null)
        {
            hintLetter.IsChecked = true;
        }
        await Task.CompletedTask;
    }

    public async Task Reset()
    {
        Letters = Array.Empty<GameLetter>();
        GuessWord = GameWord.Empty;
        await Task.CompletedTask;
    }

    public void ToggleLetter(GameLetter letter)
    {
        if (letter.IsChecked)
        {
            var idx = _userWordLetters.IndexOf(letter);
            if (idx >= 0)
            {
                _userWordLetters[idx] = null;
            }
        }
        else
        {
            var idx = _userWordLetters.IndexOf(null);
            if (idx >= 0)
            {
                _userWordLetters[idx] = letter;
            }
            else
            {
                if (_userWordLetters.Count >= GuessWord.Word.Length)
                {
                    return;
                }
                _userWordLetters.Add(letter);
            }
        }
        letter.IsChecked = !letter.IsChecked;
        UpdateUserWord();
    }

    public void RemoveLastLetter()
    {
        if (_userWordLetters.Count > 0)
        {
            _userWordLetters.Remove(_userWordLetters.Last());
            UpdateUserWord();
        }
    }

    public void ClearUserWord()
    {
        foreach (var letter in Letters)
        {
            letter.IsChecked = false;
        }
        _userWordLetters.Clear();
        UpdateUserWord();
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

    private void UpdateUserWord()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var letter in _userWordLetters)
        {
            sb.Append(letter?.Letter ?? "_");
        }
        if (GuessWord.Word.Length - sb.Length > 0)
        {
            sb.Append(new string('_', GuessWord.Word.Length - sb.Length));
        }
        UserWord = sb.ToString();
    }
}