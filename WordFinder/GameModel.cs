using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder;

public partial class GameModel : ObservableObject
{
    private const int GridSize = 5;
    private WordsDatabase _db;
    private GameTimer _gameTimer;
    private WordFitter _wordFitter;
    public GameModel(WordsDatabase db, WordFitter wordFitter, GameTimer gameTimer)
    {
        _db = db;
        _wordFitter = wordFitter;
        _gameTimer = gameTimer;
        _gameTimer.PropertyChanged += (s, e) => OnPropertyChanged(e);
        _gameTimer.TimeOver += OnTimeOver;

        GuessWord = GameWord.Empty;
        Letters = Enumerable.Range(0, GridSize * GridSize).Select(r => new GameLetter("")).ToArray();
    }

    private List<GameLetter> _userWordLetters = new List<GameLetter>();

    public event EventHandler GameOver;

    [ObservableProperty] private GameWord _guessWord;
    [ObservableProperty] private string _userWord;
    [ObservableProperty] private GameLetter[] _letters;
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _hintsLeft;
    [ObservableProperty] private int _gameDuration;

    public TimeSpan TimeLeft => _gameTimer.TimeLeft;

    public async Task Next()
    {
        bool success = false;
        do
        {
            GuessWord = await _db.GameRandomGameWord();

            if (GuessWord is null)
            {
                await _db.ResetIsPlayed();
                success = false;
                continue;
            }
            else
            {
                await _db.SetIsPlayed(GuessWord.Id, true);
            }

            _userWordLetters.Clear();
            UpdateUserWord();
            success = await CreateGameField();
        } while (!success);

        HintsLeft = GuessWord.Word.Length - 2;
    }

    public void HighlightUserLetters()
    {
        foreach (var letter in _userWordLetters.Where(l => l is not null && l.IsMainLetter))
            letter.IsFixed = true;
    }

    public async Task Hint()
    {
        if (HintsLeft <= 0)
            return;

        HintsLeft--;
        await RemoveWrongLetters();

        var hintLetter = Letters.Where(el => el.IsMainLetter && !el.IsChecked)
               .OrderBy(el => el.LetterIndex)
               .FirstOrDefault();

        if (hintLetter is not null)
        {
            await ToggleLetter(hintLetter);
            hintLetter.IsFixed = true;
            foreach (var letter in _userWordLetters)
            {
                if (letter == hintLetter)
                {
                    break;
                }
                else
                {
                    // mark previous letters to be fixed
                    letter.IsFixed = true;
                }
            }
        }
        await Task.CompletedTask;
    }

    public async Task Reset()
    {
        _gameTimer.Stop();

        foreach (var letter in Letters.Where(l => l.IsChecked))
            letter.IsChecked = false;

        UserWord = string.Empty;
        GuessWord = GameWord.Empty;
        Score = 0;

        await Task.CompletedTask;
    }

    public async Task ToggleLetter(GameLetter letter)
    {
        if (letter.IsFixed)
            return;

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

    public async Task RemoveLastLetter()
    {
        if (_userWordLetters.Count > 0)
        {
            var lastLetter = _userWordLetters.LastOrDefault(el => el is not null);
            if (lastLetter is not null && !lastLetter.IsFixed)
            {
                await ToggleLetter(lastLetter);
                UpdateUserWord();
            }
        }
    }

    public async Task ClearUserWord()
    {
        foreach (var letter in _userWordLetters.ToArray())
        {
            if (letter is null)
                continue;

            await ToggleLetter(letter);
        }
    }

    private async Task<bool> CreateGameField()
    {
        _wordFitter.Initialize(GridSize);
        if (GuessWord is not null)
        {
            if (!_wordFitter.FitWord(GuessWord))
            {
                return false;
            }
            var noiseWords = await _db.GameRandomWords(5);
            foreach (var word in noiseWords)
            {
                if (word.Word == GuessWord.Word)
                    continue;

                _wordFitter.FitSecondaryWord(word);
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

    public async Task StartGame(int gameDuration)
    {
        GameDuration = gameDuration;
        await Next();
        _gameTimer.Start(TimeSpan.FromSeconds(15));
    }

    private void OnTimeOver(object sender, EventArgs e) => OnGameOver();

    private void OnGameOver()
    {
        GameOver?.Invoke(this, EventArgs.Empty);
    }
}