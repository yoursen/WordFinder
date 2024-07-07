using WordFinder.Interfaces;
using AVFoundation;
using Foundation;

namespace WordFinder;

public class Sound : ISound
{
    private readonly AVAudioPlayer _playerTap;
    private readonly AVAudioPlayer _playerSuccess;
    private readonly AVAudioPlayer _playerFail;
    public bool IsPlayerReady { get; init; }

    public Sound()
    {
        try
        {
            NSError error;
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Ambient, AVAudioSessionCategoryOptions.MixWithOthers, out error);

            if (error != null)
                return;

            var url = NSUrl.FromString(NSBundle.MainBundle.PathForResource("sounds/tap", "m4a"));
            _playerTap = new AVAudioPlayer(url, "m4a", out error);

            if (error != null)
                return;

            url = NSUrl.FromString(NSBundle.MainBundle.PathForResource("sounds/success", "m4a"));
            _playerSuccess = new AVAudioPlayer(url, "m4a", out _);
            _playerSuccess.Volume = 0.4f;

            url = NSUrl.FromString(NSBundle.MainBundle.PathForResource("sounds/fail", "m4a"));
            _playerFail = new AVAudioPlayer(url, "m4a", out _);
            _playerFail.Volume = 0.15f;

            IsPlayerReady = _playerTap.PrepareToPlay()
                && _playerSuccess.PrepareToPlay()
                && _playerFail.PrepareToPlay();
        }
        catch { }
    }

    public void KeyboardClick() => Play(_playerTap);
    public void Success() => Play(_playerSuccess);
    public void Fail() => Play(_playerFail);

    private void Play(AVAudioPlayer player)
    {
        if (!IsPlayerReady)
            return;

        Task.Run(() =>
        {
            try
            {
                if (player.Playing)
                {
                    player.Stop();
                }
                player.Play();
            }
            catch { }
        });
    }
}