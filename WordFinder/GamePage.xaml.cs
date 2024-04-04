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
        await _viewModel.OnNavigatedTo();
    }

    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        await _viewModel.OnNavigatedFrom();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await _viewModel.Next();
    }

    private async void OnBackClicked(object sender, EventArgs e) => await GoBack(sender);

    private async void OnSwiped(object sender, SwipedEventArgs e) => await GoBack(sender);

    private async Task GoBack(object sender)
    {
        if (sender is Button btn)
            await btn.AnimateScale();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnLetterTapped(object sender, EventArgs e)
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