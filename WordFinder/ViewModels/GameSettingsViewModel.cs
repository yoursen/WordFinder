using WordFinder.Interfaces;
using WordFinder.Models;
using WordFinder.Services;

namespace WordFinder.ViewModels;

public class GameSettingsViewModel : BindableObject
{
    private readonly GameSettings _gameSettings;
    private readonly ISound _sound;
    private readonly LicenseService _license;
    private bool _isPurchangeInProgress;

    public bool IsPlayerReady => _sound.IsPlayerReady;
    public bool IsFree => _license.IsFree;
    public bool IsPro => _license.IsPro;

    public GameSettingsViewModel(GameSettings gameSettings, ISound sound, LicenseService license)
    {
        _gameSettings = gameSettings;
        _sound = sound;
        _license = license;
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

    public async Task Refresh()
    {
        OnPropertyChanged(nameof(IsFree));
        OnPropertyChanged(nameof(IsPro));
        await Task.CompletedTask;
    }
}