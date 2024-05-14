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
    private readonly GameModel _gameModel;
    private readonly IPopupService _popupService;
    private readonly GameSettings _gameSettings;

    public GamePageViewModel(GameModel gameModel, IPopupService popupService, GameSettings gameSettings)
    {
        _gameModel = gameModel;
        _popupService = popupService;
        _gameSettings = gameSettings;
    }

    public GameLetter[] Letters => _gameModel.Letters;
    public GameWord GuessWord => _gameModel.GuessWord;
    public string UserWord => _gameModel.UserWord;
    public int Score => _gameModel.Score;
    public int HintsLeft => _gameModel.HintsLeft;
    public int GameDuration { get; set; }
    public TimeSpan TimeLeft => _gameModel.TimeLeft;
    public TimeSpan HintPenaltyTimeSpan => _gameModel.HintPenaltyTimeSpan;
    public bool IsFreeplayMode => _gameModel.IsFreeplayMode;

    public void SuspendGame() => _gameModel.SuspendGame();
    public void ResumeGame() => _gameModel.ResumeGame();

    public async Task<bool> AskExitGame()
    {
        SuspendGame();
        var result = await _popupService.ShowPopupAsync<ExitGamePopupViewModel>();
        if (result is bool b && b)
        {
            return b;
        }
        else
        {
            ResumeGame();
            return false;
        }
    }

    public async Task Next()
    {
        await _gameModel.Next();
    }

    public async Task RevealAnswer()
    {
        await _gameModel.RevealAnswer();
    }

    public async Task Hint()
    {
        await _gameModel.Hint();
        await _gameModel.CheckWordAndDoNext();
    }
    public async Task ToggleLetter(GameLetter letter)
    {
        await _gameModel.ToggleLetter(letter);
        await _gameModel.CheckWordAndDoNext();
    }

    public async void RemoveLastLetter() => await _gameModel.RemoveLastLetter();
    public async void ClearUserWord() => await _gameModel.ClearUserWord();

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        => OnPropertyChanged(e.PropertyName);

    public async Task OnNavigatedTo()
    {
        _gameModel.PropertyChanged += OnPropertyChanged;
        _gameModel.GameOver += OnGameOver;
        WeakReferenceMessenger.Default.RegisterAll(this);

        await _gameModel.StartGame(GameDuration);

        if (_gameSettings.IsFirstGame)
        {
            _gameModel.SuspendGame();
            _gameSettings.IsFirstGame = false;
            await _popupService.ShowPopupAsync<HowToPlayPopupViewModel>();
            _gameModel.ResumeGame();
        }
    }

    public async Task OnNavigatingFrom()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        _gameModel.PropertyChanged -= OnPropertyChanged;
        _gameModel.GameOver -= OnGameOver;
        await _gameModel.Reset();
    }

    private async void OnGameOver(object sender, EventArgs e)
    {
        await _gameModel.Reset();
        await Shell.Current.GoToAsync("GameOver");
    }

    public async Task DoGameOver() => await _gameModel.DoGameOver();

    public void Receive(AppSuspendedMessage message) => _gameModel.SuspendGame();

    public void Receive(AppResumedMessage message) => _gameModel.ResumeGame();
}