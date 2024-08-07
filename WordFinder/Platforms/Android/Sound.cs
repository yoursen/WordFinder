using Android.Media;
using WordFinder.Interfaces;
using WordFinder.Models;

namespace WordFinder;

public class Sound : ISound
{
    public bool IsPlayerReady { get; set; }

    private MediaPlayer _mediaPlayerTap;
    private MediaPlayer _mediaPlayerFail;
    private MediaPlayer _mediaPlayerSuccess;
    private GameSettings _gameSettings;

    public Sound(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
        var context = Android.App.Application.Context;

        try
        {
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
        catch
        {
        }

    }

    private void Play(MediaPlayer player)
    {
        if (!_gameSettings.Click)
            return;

        if (!IsPlayerReady)
            return;

        try
        {
            player.Start();
        }
        catch
        {

        }
    }

    public void Fail() => Play(_mediaPlayerFail);
    public void KeyboardClick() => Play(_mediaPlayerTap);
    public void Success() => Play(_mediaPlayerSuccess);
}