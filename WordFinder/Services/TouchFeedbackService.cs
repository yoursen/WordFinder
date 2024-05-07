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

    public void Perform()
    {
        if (_settings.Vibrate)
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        if (_settings.Click)
            _sound.KeyboardClick();
    }
}