using WordFinder.Interfaces;
using WordFinder.Models;

namespace WordFinder.Services;

public class TouchFeedbackService
{
    private readonly GameSettings _settings;
    private readonly ISound _sound;

    public TouchFeedbackService(GameSettings settings, ISound sound)
    {
        _settings = settings;
        _sound = sound;
    }

    public void Vibrate()
    {
        if (_settings.Vibrate)
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }

    public void KeyboardClick()
    {
        if (_settings.Click)
            _sound.KeyboardClick();
    }

    public void Perform()
    {
        Vibrate();
        KeyboardClick();
    }
}