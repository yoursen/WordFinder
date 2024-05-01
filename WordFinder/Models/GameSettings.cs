namespace WordFinder.Models;

public class GameSettings
{
    public GameSettings()
    {
        _vibrate = Preferences.Default.Get(nameof(Vibrate), true);
        _click = Preferences.Default.Get(nameof(Click), true);
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
}