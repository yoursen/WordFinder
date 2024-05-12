using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Interfaces;
using WordFinder.Models;
using WordFinder.Services;

namespace WordFinder.ViewModels;

public partial class GameSettingsViewModel : ObservableObject
{
    private readonly GameSettings _gameSettings;
    private readonly GameDatabase _db;
    private readonly ISound _sound;
    private readonly LicenseService _license;
    private readonly TouchFeedbackService _feedback;
    private bool _isPurchangeInProgress;

    public bool IsPlayerReady => _sound.IsPlayerReady;
    public bool IsFree => _license.IsFree;
    public bool IsPro => _license.IsPro;
    [ObservableProperty] private int _totalWordsPro = 512;
    public ICommand RestorePurchaseCommand { get; }

    public GameSettingsViewModel(GameSettings gameSettings, ISound sound, LicenseService license, 
        TouchFeedbackService feedback, GameDatabase db)
    {
        _gameSettings = gameSettings;
        _sound = sound;
        _license = license;
        _feedback = feedback;
        _db = db;

        RestorePurchaseCommand = new Command(RestorePurchaseCommandHandler);
    }

    public bool Vibrate
    {
        get => _gameSettings.Vibrate;
        set => _gameSettings.Vibrate = value;
    }

    public bool Click
    {
        get => _gameSettings.Click;
        set => _gameSettings.Click = value;
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

    public async Task Refresh()
    {
        TotalWordsPro = await _db.CountWordsPro();
        OnPropertyChanged(nameof(IsFree));
        OnPropertyChanged(nameof(IsPro));
        await Task.CompletedTask;
    }
}