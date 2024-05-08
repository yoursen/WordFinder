using WordFinder.Interfaces;
using AVFoundation;
using Foundation;

namespace WordFinder;

public class Sound : ISound
{
    private AVAudioPlayer _player;
    public bool IsPlayerReady { get; init; }

    public Sound()
    {
        try
        {
            NSError error;
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Ambient, AVAudioSessionCategoryOptions.MixWithOthers, out error);

            if (error != null)
                return;

            string path = NSBundle.MainBundle.PathForResource("sounds/tap", "wav");
            var url = NSUrl.FromString(path);

            _player = new AVAudioPlayer(url, "wav", out error);

            if (error != null)
                return;

            IsPlayerReady = _player.PrepareToPlay();
        }
        catch { }
    }

    public void KeyboardClick()
    {
        if (!IsPlayerReady)
            return;

        if (_player.Playing)
            _player.Stop();

        try
        {
            _player.Play();
        }
        catch { }
    }
}