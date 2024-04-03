namespace WordFinder;

public partial class GamePage : ContentPage
{
    private GamePageViewModel _viewModel;
    public GamePage(GamePageViewModel viewModel, WordsDatabase db)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Next();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        _viewModel.Reset();
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

            _viewModel.ToggleButton(ch);
        }

        await frame.AnimateScale();
    }
}