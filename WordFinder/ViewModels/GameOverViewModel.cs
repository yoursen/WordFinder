using System.Windows.Input;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Models;
using WordFinder.Services;

namespace WordFinder.ViewModels;

public partial class GameOverViewModel : ObservableObject
{
    private readonly GameModel _gameModel;
    private readonly GameDatabase _db;
    private readonly LicenseService _license;
    private readonly IPopupService _popup;
    private readonly TouchFeedbackService _feedback;
    public ICommand RestorePurchaseCommand { get; }

    private bool _isPurchangeInProgress;
    public GameOverViewModel(GameModel gameModel, GameDatabase db, LicenseService license, IPopupService popup,
        TouchFeedbackService feedback)
    {
        _gameModel = gameModel;
        _db = db;
        _license = license;
        _popup = popup;
        _feedback = feedback;

        RestorePurchaseCommand = new Command(RestorePurchaseCommandHandler);
    }

    [ObservableProperty] private int _score;
    [ObservableProperty] private int _bestScore;
    [ObservableProperty] private bool _isRecord;
    [ObservableProperty] private int _gameDuration;
    [ObservableProperty] private int _totalWordsNotAnswered;
    [ObservableProperty] private int _totalWordsPro = 512;

    public string BuyProVersionText
        => $"{(TotalWordsNotAnswered <= 20 ? "Only " : string.Empty)}{TotalWordsNotAnswered} not guessed words left in the free version. Unlock an additional {TotalWordsPro} words in the premium version and keep the excitement going!'";

    public bool IsFree => _license.IsFree;
    public async Task Refresh()
    {
        var gameScore = await _db.GetLastGameScore();
        var bestScore = await _db.GetBestGameScore(_gameModel.GameDuration);

        GameDuration = gameScore.GameDuration;
        Score = gameScore.Score;
        BestScore = bestScore?.Score ?? Score;
        IsRecord = Score > BestScore;

        TotalWordsNotAnswered = await _db.CountWordsNotAnswered();
        TotalWordsPro = await _db.CountWordsPro();
        OnPropertyChanged(nameof(IsFree));
        OnPropertyChanged(nameof(BuyProVersionText));

        if (TotalWordsNotAnswered == 0)
        {
            await _popup.ShowPopupAsync<NoWordsPopupViewModel>();
            await _db.ResetIsAnswered();
            await _db.ResetIsPlayed();
        }

        await Task.CompletedTask;
        return;
    }

    public void OnNavigatingFrom()
    {
        GameDuration = 0;
        BestScore = 0;
        Score = 0;
        IsRecord = false;
    }

    public async void BuyPro()
    {
        if (_isPurchangeInProgress)
            return;

        try
        {
            _isPurchangeInProgress = true;
            await _license.BuyPro();
            await Refresh();
        }
        finally
        {
            _isPurchangeInProgress = false;
        }
    }

    private async void RestorePurchaseCommandHandler(object obj)
    {
        _feedback.Perform();

        if (_isPurchangeInProgress)
            return;

        try
        {
            _isPurchangeInProgress = true;
            await _license.RestoreProLicense();
            await Refresh();
        }
        finally
        {
            _isPurchangeInProgress = false;
        }
    }
}