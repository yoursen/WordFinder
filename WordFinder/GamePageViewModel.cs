using System.ComponentModel;

namespace WordFinder;

[QueryProperty(nameof(GameDuration), "GameDuration")]
public class GamePageViewModel : BindableObject
{
    private GameModel _gameModel;
    private GameTimer _gameTimer;

    public GamePageViewModel(GameModel gameModel, GameTimer gameTimer)
    {
        _gameModel = gameModel;
        _gameTimer = gameTimer;
    }

    public GameLetter[] Letters => _gameModel.Letters;
    public GameWord GuessWord => _gameModel.GuessWord;
    public string UserWord => _gameModel.UserWord;
    public int Score => _gameModel.Score;
    public int HintsLeft => _gameModel.HintsLeft;

    public int GameDuration { get; set; }
    public TimeSpan TimeLeft => _gameTimer.TimeLeft;


    public async Task Reset()
    {
        _gameTimer.Stop();
        await _gameModel.Reset();
    }
    public async Task Next() => await _gameModel.Next();
    public async Task Hint()
    {
        await _gameModel.Hint();
        await CheckWordAndDoNext();

    }
    public async Task ToggleLetter(GameLetter letter)
    {
        await _gameModel.ToggleLetter(letter);
        await CheckWordAndDoNext();
    }

    public async void RemoveLastLetter() => await _gameModel.RemoveLastLetter();
    public async void ClearUserWord() => await _gameModel.ClearUserWord();

    private async Task CheckWordAndDoNext()
    {
        if (_gameModel.IsGuessWordCorrect())
        {
            _gameModel.Score++;
            _gameModel.HighlightUserLetters();
            await Task.Delay(100);
            await Next();
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        => OnPropertyChanged(e.PropertyName);

    public async Task OnNavigatedTo()
    {
        _gameModel.PropertyChanged += OnPropertyChanged;
        _gameTimer.PropertyChanged += OnPropertyChanged;

        await _gameModel.Next();
        _gameTimer.Start(TimeSpan.FromMinutes(GameDuration));
    }

    public void OnNavigatedFrom()
    {
        _gameModel.PropertyChanged -= OnPropertyChanged;
        _gameTimer.PropertyChanged -= OnPropertyChanged;
    }
}