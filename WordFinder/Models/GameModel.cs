using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Services;

namespace WordFinder.Models;

public partial class GameModel : ObservableObject
{
    private const int GridSize = 5;
    private GameDatabase _db;
    private GameTimer _gameTimer;
    private WordFitter _wordFitter;
    public TimeSpan HintPenaltyTimeSpan { get; init; } = TimeSpan.FromSeconds(5);
    private AwaitableMessageService _ams;
    public GameModel(GameDatabase db, WordFitter wordFitter, GameTimer gameTimer, AwaitableMessageService ams)
    {
        _db = db;
        _wordFitter = wordFitter;
        _gameTimer = gameTimer;
        _gameTimer.PropertyChanged += (s, e) => OnPropertyChanged(e);
        _gameTimer.TimeOver += OnTimeOver;
        _ams = ams;

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
    [ObservableProperty] private bool _isFreeplayMode;

    public TimeSpan TimeLeft => _gameTimer.TimeLeft;

    public void SuspendGame() => _gameTimer.Stop();
    public void ResumeGame() => _gameTimer.Start();

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
                    letter.IsFixed = true;
                }
            }
        }

        if (!IsFreeplayMode)
        {
            _gameTimer.AddPenalty(HintPenaltyTimeSpan);
            await _ams.Send("PenaltyApplied", HintPenaltyTimeSpan);
        }
    }

    public async Task Reset()
    {
        _gameTimer.Reset();

        foreach (var letter in Letters.Where(l => l.IsChecked))
            letter.IsChecked = false;

        UserWord = string.Empty;
        GuessWord = GameWord.Empty;
        Score = 0;
        UpdateUserWord();

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

    public async Task RevealAnswer()
    {
        _userWordLetters.Clear();
        foreach (var letter in Letters.OrderBy(l=>l.LetterIndex))
        {
            letter.IsFixed = false;
            letter.IsChecked = false;
            if (!letter.IsChecked && letter.IsMainLetter)
                await ToggleLetter(letter);
        }

        UpdateUserWord();
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

    public async Task StartGame(int gameDurationSec)
    {
        IsFreeplayMode = gameDurationSec <= 0;
        GameDuration = gameDurationSec;
        await Next();
        _gameTimer.Start(TimeSpan.FromMinutes(gameDurationSec));
    }

    private async void OnTimeOver(object sender, EventArgs e) => await DoGameOver();

    public async Task DoGameOver()
    {
        var gameScore = new GameScore()
        {
            Score = Score,
            GameDuration = GameDuration
        };
        await _db.AddGameScore(gameScore);

        GameOver?.Invoke(this, EventArgs.Empty);
    }
}