using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder.Models;

public partial class GameTimer : ObservableObject
{
    private IDispatcherTimer _timer;
    public event EventHandler TimeOver;
    private TimeSpan OneSecondTimeSpan = TimeSpan.FromSeconds(1);
    private TimeSpan PenalyTimeSpan = TimeSpan.Zero;

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
        Start();
    }

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();

    public void Reset()
    {
        Stop();
        TimeLeft = new TimeSpan();
    }

    public void AddPenalty(TimeSpan ts)
    {
        lock (this)
        {
            PenalyTimeSpan += ts;
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        TimeSpan fee = TimeSpan.Zero;
        lock (this)
        {
            fee = PenalyTimeSpan;
            PenalyTimeSpan = TimeSpan.Zero;
        }

        var timeLeft = TimeLeft - OneSecondTimeSpan - fee;
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