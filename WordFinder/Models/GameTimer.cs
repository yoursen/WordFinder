using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder.Models;

public partial class GameTimer : ObservableObject
{
    private IDispatcherTimer _timer;
    public event EventHandler TimeOver;

    public GameTimer()
    {
        _timer = Dispatcher.GetForCurrentThread().CreateTimer();
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Interval = TimeSpan.FromSeconds(1);
    }

    [ObservableProperty] TimeSpan _timeLeft;

    public void Start(TimeSpan timeLeft)
    {
        TimeLeft = timeLeft;
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        var timeLeft = TimeLeft - TimeSpan.FromSeconds(1);
        if (timeLeft.TotalMilliseconds < 0)
            timeLeft = TimeSpan.FromSeconds(0);
            
        TimeLeft = timeLeft;
        if (TimeLeft.TotalMilliseconds <= 0)
        {
            Stop();
            TimeOver?.Invoke(this, EventArgs.Empty);
        }
    }
}