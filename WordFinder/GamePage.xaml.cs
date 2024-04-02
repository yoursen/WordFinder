namespace WordFinder;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

[QueryProperty(nameof(Count), "Count")]
public partial class GamePage : ContentPage
{
    private WordsDatabase _db;
    public GamePage(WordsDatabase db)
    {
        InitializeComponent();
        _db = db;

        RefreshGameField();

        BindingContext = this;
        //IsBusy = true;
    }

    private void RefreshGameField() => Letters = GreateGameField();

    private GameLetter[] GreateGameField()
    {
        var field = new List<GameLetter>();
        for (int i = 0; i < 25; i++)
        {
            var letterStr = ((char)Random.Shared.Next(65, 90)).ToString();
            var gameLetter = new GameLetter(letterStr);
            field.Add(gameLetter);
        }
        return field.ToArray();
    }

    public static readonly BindableProperty LettersProperty =
            BindableProperty.Create(nameof(Letters), typeof(GameLetter[]), typeof(GamePage), default(GameLetter[]));

    public GameLetter[] Letters
    {
        get => (GameLetter[])GetValue(LettersProperty);
        set => SetValue(LettersProperty, value);
    }

    public static readonly BindableProperty CountProperty =
            BindableProperty.Create(nameof(Count), typeof(int), typeof(GamePage), 0);

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(string), typeof(GamePage), "");

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly BindableProperty RecentCountersProperty =
            BindableProperty.Create(nameof(RecentCounters), typeof(List<CountItem>), typeof(GamePage), new List<CountItem>());

    public List<CountItem> RecentCounters
    {
        get => (List<CountItem>)GetValue(RecentCountersProperty);
        set => SetValue(RecentCountersProperty, value);
    }

    // private async Task Refresh()
    // {
    //     RecentCounters = await _db.GetItemsAsync();
    //     OnPropertyChanged(nameof(RecentCounters));
    // }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
       
        // IsBusy = true;
        // Status = "Loading...";
        // await Refresh();
        // Status = "Loaded";
        //IsBusy = false;

        //await ShowToast();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        RefreshGameField();
    }

    private async void OnBackClicked(object sender, EventArgs e) => await GoBack(sender);

    private async void OnSwiped(object sender, SwipedEventArgs e) => await GoBack(sender);

    private async Task GoBack(object sender)
    {
        if(sender is Button btn)
            await btn.AnimateScale();
        await Shell.Current.GoToAsync("..");
    }

    // private async Task ShowToast(string msg = null)
    // {
    //     CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    //     string text = msg ?? "Great move!";
    //     ToastDuration duration = ToastDuration.Short;
    //     double fontSize = 14;

    //     var toast = Toast.Make(text, duration, fontSize);

    //     await toast.Show(cancellationTokenSource.Token);
    // }

    private async void OnTapped(object sender, EventArgs e)
    {
        var frame = sender as ContentView;
        if (frame is null)
            return;

        if (frame.Parent is CharButtonView ch)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            ch.IsChecked = !ch.IsChecked;
            Status += ch.Title;
        }

        await frame.AnimateScale();
    }
}