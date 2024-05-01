using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GamePage : ContentPage
{
    private GamePageViewModel _viewModel;
    private AwaitableMessageService _ams;
    private TouchFeedbackService _feedback;
    public GamePage(GamePageViewModel viewModel, GameDatabase db, AwaitableMessageService ams,
        TouchFeedbackService touchFeedbackService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _ams = ams;
        _feedback = touchFeedbackService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.OnNavigatedTo();

        _ams.Register("WrongTextEntered", OnWrongTextEntered);
        _ams.Register("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Register("PenaltyApplied", OnPenaltyApplied);
    }

    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        await _viewModel.OnNavigatingFrom();
        _ams.Unregister("WrongTextEntered", OnWrongTextEntered);
        _ams.Unregister("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Unregister("PenaltyApplied", OnPenaltyApplied);
    }

    private async Task OnCorrectTextEntered(object args) => await UserTextLabel.AnimateScale(1.15);

    private async Task OnWrongTextEntered(object args)
    {
        UserTextLabel.AnimateShake();
        await Task.CompletedTask;
    }

    private async Task OnPenaltyApplied(object args) => await PenaltyLabel.AnimateDrop();

    private async void OnNextClicked(object sender, EventArgs e)
    {
        //await (sender as VisualElement).AnimateScale();
        _feedback.DoFeedback();
        await _viewModel.Next();
    }

    private async void OnHintClicked(object sender, EventArgs e)
    {
        //await (sender as VisualElement).AnimateScale();
        _feedback.DoFeedback();
        await _viewModel.Hint();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        _feedback.DoFeedback();
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
        var button = sender as Button;
        if (button is null)
            return;

        if (button.Parent?.BindingContext is GameLetter letter)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await _viewModel.ToggleLetter(letter);
        }

        _feedback.DoFeedback();
        await button.AnimateScale();
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _feedback.DoFeedback();
        _viewModel.ClearUserWord();

    }
    private void OnClearLastLetter(object sender, EventArgs e)
    {
        _feedback.DoFeedback();
        _viewModel.RemoveLastLetter();
    }

}