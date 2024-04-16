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

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        _viewModel.OnNavigatedFrom();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await _viewModel.Next();
    }

    private async void OnHintClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await _viewModel.Hint();
    }

    private async void OnBackClicked(object sender, EventArgs e) => await GoBack(sender);

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        await Task.CompletedTask;
    }

    private void OnSwipedLeft(object sender, SwipedEventArgs e) => _viewModel.RemoveLastLetter();

    private async Task GoBack(object sender)
    {
        if (sender is Button btn)
            await btn.AnimateScale();

        await _viewModel.Reset();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnLetterTapped(object sender, EventArgs e)
    {
        var frame = sender as ContentView;
        if (frame is null)
            return;

        if (frame.Parent?.BindingContext is GameLetter letter)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await _viewModel.ToggleLetter(letter);
        }

        await frame.AnimateScale();
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        _viewModel.ClearUserWord();
    }
}