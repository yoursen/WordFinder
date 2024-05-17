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
        {
#if __ANDROID__
            Vibration.Vibrate(40);
#else
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
#endif
        }
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