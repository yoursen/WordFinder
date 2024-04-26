using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GamePage : ContentPage
{
    private GamePageViewModel _viewModel;
    private AwaitableMessageService _ams;
    public GamePage(GamePageViewModel viewModel, GameDatabase db, AwaitableMessageService ams)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _ams = ams;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.OnNavigatedTo();

        _ams.Register("WrongTextEntered", OnWrongTextEntered);
        _ams.Register("CorrectTextEntered", OnCorrectTextEntered);
    }

    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        await _viewModel.OnNavigatingFrom();
        _ams.Unregister("WrongTextEntered", OnWrongTextEntered);
        _ams.Unregister("CorrectTextEntered", OnCorrectTextEntered);
    }

    private async Task OnCorrectTextEntered(object args)
    {
        await UserTextLabel.AnimateScale(1.15);
    }

    private async Task OnWrongTextEntered(object args)
    {
        UserTextLabel.AnimateShake();
        await Task.CompletedTask;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        //await (sender as VisualElement).AnimateScale();
        await _viewModel.Next();
    }

    private async void OnHintClicked(object sender, EventArgs e)
    {
        //await (sender as VisualElement).AnimateScale();
        await _viewModel.Hint();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        var exit = await _viewModel.AskExitGame();
        if (exit)
            await Shell.Current.GoToAsync("///MainPage");
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        await Task.CompletedTask;
    }

    private void OnSwipedLeft(object sender, SwipedEventArgs e) => _viewModel.RemoveLastLetter();

    private async void OnLetterClicked(object sender, EventArgs e)
    {
        var frame = sender as Button;
        if (frame is null)
            return;

        if (frame.Parent?.BindingContext is GameLetter letter)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await _viewModel.ToggleLetter(letter);
        }

        await frame.AnimateScale();
    }

    private void OnClearClicked(object sender, EventArgs e) => _viewModel.ClearUserWord();
    private void OnClearLastLetter(object sender, EventArgs e) => _viewModel.RemoveLastLetter();
}