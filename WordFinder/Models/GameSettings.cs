namespace WordFinder.Models;

public class GameSettings
{
    public GameSettings()
    {
        _vibrate = Preferences.Default.Get(nameof(Vibrate), true);
        _click = Preferences.Default.Get(nameof(Click), true);
        _isFirstGame = Preferences.Default.Get(nameof(IsFirstGame), true);
        _language = (GameLanguage)Preferences.Default.Get(nameof(Language), (int)GameLanguage.English);
    }

    private GameLanguage _language;
    public GameLanguage Language
    {
        get => _language;
        set
        {
            _language = value;
            Preferences.Default.Set(nameof(Language), (int)_language);
        }
    }

    private bool _vibrate;
    public bool Vibrate
    {
        get => _vibrate;
        set
        {
            _vibrate = value;
            Preferences.Default.Set(nameof(Vibrate), _vibrate);
        }
    }

    private bool _click;
    public bool Click
    {
        get => _click;
        set
        {
            _click = value;
            Preferences.Default.Set(nameof(Click), _click);
        }
    }

    private bool _isFirstGame;
    public bool IsFirstGame
    {
        get => _isFirstGame;
        set
        {
            _isFirstGame = value;
            Preferences.Default.Set(nameof(IsFirstGame), _isFirstGame);
        }
    }
}