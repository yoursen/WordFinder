using WordFinder.Interfaces;
using WordFinder.Models;

namespace WordFinder.ViewModels;

public class GameSettingsViewModel : BindableObject
{
    private readonly GameSettings _gameSettings;
    private readonly ISound _sound;

    public bool IsPlayerReady => _sound.IsPlayerReady;

    public GameSettingsViewModel(GameSettings gameSettings, ISound sound)
    {
        _gameSettings = gameSettings;
        _sound = sound;
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
}