namespace WordFinder;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class GamePage : ContentPage
{
    private WordsDatabase _db;
    private GamePageViewModel _viewModel;
    public GamePage(GamePageViewModel viewModel, WordsDatabase db)
    {
        InitializeComponent();
        _db = db;
        _viewModel = viewModel;

        RefreshGameField();

        BindingContext = this;
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

    public static readonly BindableProperty GuessWordProperty =
            BindableProperty.Create(nameof(GuessWord), typeof(string), typeof(GamePage), "");

    public string GuessWord
    {
        get => (string)GetValue(GuessWordProperty);
        set => SetValue(GuessWordProperty, value);
    }

    public static readonly BindableProperty UserWordProperty =
            BindableProperty.Create(nameof(UserWord), typeof(string), typeof(GamePage), "");

    public string UserWord
    {
        get => (string)GetValue(UserWordProperty);
        set => SetValue(UserWordProperty, value);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var gameWord = await _db.GetRandomWord();
        GuessWord = gameWord.Description;
        UserWord = new string('_', gameWord.Word.Length);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        RefreshGameField();
        GuessWord = string.Empty;
        UserWord = string.Empty;
    }

    private async void OnBackClicked(object sender, EventArgs e) => await GoBack(sender);

    private async void OnSwiped(object sender, SwipedEventArgs e) => await GoBack(sender);

    private async Task GoBack(object sender)
    {
        if (sender is Button btn)
            await btn.AnimateScale();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnTapped(object sender, EventArgs e)
    {
        var frame = sender as ContentView;
        if (frame is null)
            return;

        if (frame.Parent is CharButtonView ch)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            ch.IsChecked = !ch.IsChecked;

            if(UserWord.Contains("_"))
                UserWord = string.Empty;

            UserWord += ch.Title;
        }

        await frame.AnimateScale();
    }
}