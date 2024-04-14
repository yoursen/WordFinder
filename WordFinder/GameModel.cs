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
        await RemoveWrongLetters();

        var gameLetter = Letters.Where(el => el.IsMainLetter && !el.IsChecked)
               .OrderBy(el => el.LetterIndex)
               .FirstOrDefault();

        if (gameLetter is not null)
            await ToggleLetter(gameLetter);

        await Task.CompletedTask;
    }

    public async Task Reset()
    {
        Letters = Array.Empty<GameLetter>();
        GuessWord = GameWord.Empty;
        await Task.CompletedTask;
    }

    public async Task ToggleLetter(GameLetter letter)
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
            var firstNullIndex = _userWordLetters.IndexOf(null);
            if (firstNullIndex >= 0)
            {
                _userWordLetters[firstNullIndex] = letter;
            }
            else
            {
                if (_userWordLetters.Count >= GuessWord.Word.Length)
                    return;
                else
                    _userWordLetters.Add(letter);
            }
        }
        letter.IsChecked = !letter.IsChecked;
        UpdateUserWord();
        await Task.CompletedTask;
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

    public bool IsGuessWordCorrect() => string.Compare(GuessWord.Word, UserWord, StringComparison.OrdinalIgnoreCase) == 0;

    private async Task<bool> RemoveWrongLetters()
    {
        var isRemoved = false;

        // remove all toggled letters that are not main
        foreach (var letter in _userWordLetters.Where(l => l?.IsChecked == true && !l.IsMainLetter).ToArray())
        {
            isRemoved = true;
            await ToggleLetter(letter);
        }

        // check for position of letters
        List<GameLetter> toRemove = new();
        for (int i = 0; i < _userWordLetters.Count; i++)
        {
            if (_userWordLetters[i] is null)
                continue;

            if (string.Compare(GuessWord.Word[i].ToString(), _userWordLetters[i].Letter, StringComparison.OrdinalIgnoreCase) != 0)
                toRemove.Add(_userWordLetters[i]);
        }

        foreach (var letter in toRemove)
        {
            isRemoved = true;
            await ToggleLetter(letter);
        }

        return isRemoved;
    }
}