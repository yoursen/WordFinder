using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Models;
using WordFinder.Services;

namespace WordFinder.ViewModels;

public partial class GameBestScoreViewModel : ObservableObject
{
    private readonly GameDatabase _db;
    private readonly LicenseService _license;
    private readonly TouchFeedbackService _feedback;

    private bool _isPurchangeInProgress;
    public ICommand RestorePurchaseCommand { get; }

    [ObservableProperty] private int _score2min;
    [ObservableProperty] private int _score5min;
    [ObservableProperty] private int _score10min;
    [ObservableProperty] private int _scoreFreeplay;
    [ObservableProperty] private int _totalWords;
    [ObservableProperty] private int _totalWordsAnswered;
    [ObservableProperty] private int _totalWordsPro = 512;

    public bool IsFree => _license.IsFree;

    public GameBestScoreViewModel(GameDatabase db, LicenseService license, TouchFeedbackService feedback)
    {
        _db = db;
        _license = license;
        _feedback = feedback;
        RestorePurchaseCommand = new Command(RestorePurchaseCommandHandler);
    }

    public async Task Refresh()
    {
        Score2min = (await _db.GetBestGameScore(2))?.Score ?? 0;
        Score5min = (await _db.GetBestGameScore(5))?.Score ?? 0;
        Score10min = (await _db.GetBestGameScore(10))?.Score ?? 0;
        ScoreFreeplay = (await _db.GetBestGameScore(-1))?.Score ?? 0;
        TotalWords = await _db.CountWords();
        TotalWordsAnswered = await _db.CountWordsAnswered();
        TotalWordsPro = await _db.CountWordsPro();
        OnPropertyChanged(nameof(IsFree));
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