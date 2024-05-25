using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Services;

namespace WordFinder.Models;

public partial class GameModel : ObservableObject
{
    private const int GridSize = 5;
    private readonly GameDatabase _db;
    private readonly GameSettings _gameSettings;
    private readonly GameTimer _gameTimer;
    private readonly WordFitter _wordFitter;
    private readonly GameHintsModel _hintsModel;
    private readonly AwaitableMessageService _ams;
    private readonly TouchFeedbackService _touchFeedback;
    public TimeSpan HintPenaltyTimeSpan { get; init; } = TimeSpan.FromSeconds(5);

    public GameModel(GameDatabase db, WordFitter wordFitter, GameTimer gameTimer, AwaitableMessageService ams
    , TouchFeedbackService touchFeedback, GameHintsModel hintsModel, GameSettings gameSettings)
    {
        _db = db;
        _wordFitter = wordFitter;
        _gameSettings = gameSettings;
        _touchFeedback = touchFeedback;
        _gameTimer = gameTimer;
        _gameTimer.PropertyChanged += (s, e) => OnPropertyChanged(e);
        _gameTimer.TimeOver += OnTimeOver;
        _ams = ams;
        _hintsModel = hintsModel;
        _hintsModel.PropertyChanged += (s, e) => OnPropertyChanged(e);

        GuessWord = GameWord.Empty;
        Letters = Enumerable.Range(0, GridSize * GridSize).Select(r => new GameLetter("")).ToArray();
    }

    private List<GameLetter> _userWordLetters = new List<GameLetter>();

    public event EventHandler GameOver;

    [ObservableProperty] private GameWord _guessWord;
    [ObservableProperty] private string _userWord;
    [ObservableProperty] private GameLetter[] _letters;
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _gameDuration;
    [ObservableProperty] private bool _isFreeplayMode;

    public int HintsLeft => _hintsModel.HintsLeft;
    public TimeSpan TimeLeft => _gameTimer.TimeLeft;

    public void SuspendGame() => _gameTimer.Stop();
    public void ResumeGame() => _gameTimer.Start();

    public async Task Next()
    {
        bool success;
        do
        {
            GuessWord = await _db.GameRandomGameWord();

            if (GuessWord is null)
            {
                var wordsLeft = await _db.ResetIsPlayed();
                if (wordsLeft == 0)
                {
                    await DoGameOver();
                    return;
                }

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

        _hintsModel.UpdateHintsPerWord(GuessWord.Word.Length);
    }

    public void HighlightUserLetters()
    {
        foreach (var letter in _userWordLetters.Where(l => l is not null && l.IsMainLetter))
            letter.IsFixed = true;
    }

    public async Task Hint()
    {
        if (_hintsModel.UseHint() is false)
            return;

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
        if (letter.IsFixed && letter.IsChecked)
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
        foreach (var letter in _userWordLetters.ToArray().Reverse())
        {
            if (letter is null || letter.IsFixed)
                continue;

            await ToggleLetter(letter);
#if __IOS__
                    _touchFeedback.Vibrate();
#endif
            await Task.Delay(40);
        }
    }

    private async Task<bool> CreateGameField()
    {
        _wordFitter.Initialize(GridSize);
        if (GuessWord is not null)
        {
            if (!_wordFitter.FitWord(GuessWord))
            {
                _wordFitter.FitWordDummy(GuessWord);
            }
            var noiseWords = await _db.GameRandomWords(5);
            foreach (var word in noiseWords)
            {
                if (word.Word == GuessWord.Word)
                    continue;

                _wordFitter.FitSecondaryWord(word);
            }
        }
        _wordFitter.FitBlank(_gameSettings.Language);
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
        await RemoveWrongLetters();
        UpdateUserWord();

        foreach (var letter in Letters.OrderBy(l => l.LetterIndex))
        {
            letter.IsFixed = false;
            if (letter.IsMainLetter)
            {
                letter.IsFixed = true;

                if (!letter.IsChecked)
                {
#if __IOS__
                    _touchFeedback.Vibrate();
#endif
                    await Task.Delay(40);
                }

                await ToggleLetter(letter);
            }
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
        bool isGap = false;
        for (int i = 0; i < _userWordLetters.Count; i++)
        {
            if (_userWordLetters[i] is null)
            {
                isGap = true;
                continue;
            }

            if (isGap || string.Compare(GuessWord.Word[i].ToString(), _userWordLetters[i].Letter, StringComparison.OrdinalIgnoreCase) != 0)
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

    public async Task CheckWordAndDoNext()
    {
        if (IsGuessWordCorrect())
        {
            Score++;
            HighlightUserLetters();
            await _db.SetIsAnswered(GuessWord.Id, true);
            await _ams.Send("CorrectTextEntered");
            await Next();
        }
        else if (!UserWord.Contains("_"))
        {
            await _ams.Send("WrongTextEntered");
        }
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