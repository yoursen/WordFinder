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
    [ObservableProperty] private int _totalWordsPro = 583;
    public ICommand RestorePurchaseCommand { get; }
    public ICommand ChangeLanguageCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    public GameSettingsViewModel(GameSettings gameSettings, ISound sound, LicenseService license,
        TouchFeedbackService feedback, GameDatabase db)
    {
        _gameSettings = gameSettings;

        _sound = sound;
        _license = license;
        _feedback = feedback;
        _db = db;

        RestorePurchaseCommand = new Command(RestorePurchaseCommandHandler);
        ChangeLanguageCommand = new Command(ChangeLanguageCommandHandler);
        ChangeThemeCommand = new Command(ChangeThemeCommandHandler);
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

    public GameTheme Theme
    {
        get => _gameSettings.Theme;
        set
        {
            _gameSettings.Theme = value;
            OnPropertyChanged(nameof(Theme));
        }
    }

    public GameLanguage Language
    {
        get => _gameSettings.Language;
        set
        {
            _gameSettings.Language = value;
            OnPropertyChanged(nameof(Language));
        }
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

    private void ChangeLanguageCommandHandler(object obj)
    {
        _feedback.Perform();
        Language = Language == GameLanguage.English ? GameLanguage.Ukrainian : GameLanguage.English;
    }

    private void ChangeThemeCommandHandler(object obj)
    {
        _feedback.Perform();
        switch (Theme)
        {
            case GameTheme.Auto:
                Theme = GameTheme.Light;
                break;
            case GameTheme.Light:
                Theme = GameTheme.Dark;
                break;
            case GameTheme.Dark:
                Theme = GameTheme.Auto;
                break;
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