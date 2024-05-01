using WordFinder.Models;

namespace WordFinder.ViewModels;

public class GameSettingsViewModel : BindableObject
{
    private readonly GameSettings _gameSettings;

    public GameSettingsViewModel(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
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