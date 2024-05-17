using Android.Media;
using WordFinder.Interfaces;

namespace WordFinder;

public class Sound : ISound
{
    public bool IsPlayerReady { get; set; }

    private MediaPlayer _mediaPlayerTap;
    private MediaPlayer _mediaPlayerFail;
    private MediaPlayer _mediaPlayerSuccess;

    public Sound()
    {
        var context = Android.App.Application.Context;

        var fd = context.Resources.Assets.OpenFd("sounds/tap.m4a");
        _mediaPlayerTap = new MediaPlayer();
        _mediaPlayerTap.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
        _mediaPlayerTap.Prepare();

        fd = context.Resources.Assets.OpenFd("sounds/success.m4a");
        _mediaPlayerSuccess = new MediaPlayer();
        _mediaPlayerSuccess.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
        _mediaPlayerSuccess.Prepare();
        _mediaPlayerSuccess.SetVolume(0.4f, 0.4f);

        fd = context.Resources.Assets.OpenFd("sounds/fail.m4a");
        _mediaPlayerFail = new MediaPlayer();
        _mediaPlayerFail.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
        _mediaPlayerFail.Prepare();
        _mediaPlayerFail.SetVolume(0.15f, 0.15f);

        IsPlayerReady = true;
    }

    private void Play(MediaPlayer player)
    {
        if (!IsPlayerReady)
            return;

        if (player.IsPlaying)
            player.Stop();

        player.Start();
    }

    public void Fail() => Play(_mediaPlayerFail);
    public void KeyboardClick() => Play(_mediaPlayerTap);
    public void Success() => Play(_mediaPlayerSuccess);
}