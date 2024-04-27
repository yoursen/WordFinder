using System.ComponentModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using WordFinder.Messages;
using WordFinder.Models;
using WordFinder.Services;

namespace WordFinder.ViewModels;

[QueryProperty(nameof(GameDuration), "GameDuration")]
public class GamePageViewModel : BindableObject, IRecipient<AppSuspendedMessage>, IRecipient<AppResumedMessage>
{
    private GameModel _gameModel;
    private IPopupService _popupService;
    private AwaitableMessageService _ams;

    public GamePageViewModel(GameModel gameModel, IPopupService popupService, AwaitableMessageService ams)
    {
        _gameModel = gameModel;
        _popupService = popupService;
        _ams = ams;
    }

    public GameLetter[] Letters => _gameModel.Letters;
    public GameWord GuessWord => _gameModel.GuessWord;
    public string UserWord => _gameModel.UserWord;
    public int Score => _gameModel.Score;
    public int HintsLeft => _gameModel.HintsLeft;
    public int GameDuration { get; set; }
    public TimeSpan TimeLeft => _gameModel.TimeLeft;
    public TimeSpan HintPenaltyTimeSpan => _gameModel.HintPenaltyTimeSpan;

    public async Task<bool> AskExitGame()
    {
        _gameModel.SuspendGame();
        var result = await _popupService.ShowPopupAsync<ExitGamePopupViewModel>();
        if (result is bool b && b)
        {
            return b;
        }
        else
        {
            _gameModel.ResumeGame();
            return false;
        }
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
            await _ams.Send("CorrectTextEntered");
            await Next();
        }
        else if (!UserWord.Contains("_"))
        {
            await _ams.Send("WrongTextEntered");
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        => OnPropertyChanged(e.PropertyName);

    public async Task OnNavigatedTo()
    {
        _gameModel.PropertyChanged += OnPropertyChanged;
        _gameModel.GameOver += OnGameOver;
        await _gameModel.StartGame(GameDuration);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public async Task OnNavigatingFrom()
    {
        await _gameModel.Reset();
        _gameModel.PropertyChanged -= OnPropertyChanged;
        _gameModel.GameOver -= OnGameOver;
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private async void OnGameOver(object sender, EventArgs e)
    {
        await _gameModel.Reset();
        await Shell.Current.GoToAsync("GameOver");
    }

    public void Receive(AppSuspendedMessage message) => _gameModel.SuspendGame();

    public void Receive(AppResumedMessage message) => _gameModel.ResumeGame();
}