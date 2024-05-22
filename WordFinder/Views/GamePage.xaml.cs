using WordFinder.Interfaces;
using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GamePage : ContentPage, INavigationPage
{
    private readonly GamePageViewModel _viewModel;
    private readonly AwaitableMessageService _ams;
    private readonly TouchFeedbackService _feedback;
    private readonly ISound _sound;
    private readonly IBackNavigationHandler _backNavigationHandler;
    public GamePage(GamePageViewModel viewModel, AwaitableMessageService ams,
        TouchFeedbackService touchFeedbackService, ISound sound,
        IBackNavigationHandler backNavigationHandler)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _ams = ams;
        _feedback = touchFeedbackService;
        _sound = sound;
        _backNavigationHandler = backNavigationHandler;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _backNavigationHandler.Page = this;
        _ams.Register("WrongTextEntered", OnWrongTextEntered);
        _ams.Register("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Register("PenaltyApplied", OnPenaltyApplied);

        base.OnNavigatedTo(args);
        await _viewModel.OnNavigatedTo();
    }


    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        _backNavigationHandler.Page = null;
        _ams.Unregister("WrongTextEntered", OnWrongTextEntered);
        _ams.Unregister("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Unregister("PenaltyApplied", OnPenaltyApplied);

        base.OnNavigatingFrom(args);
        await _viewModel.OnNavigatingFrom();
    }

    private async Task OnCorrectTextEntered(object args)
    {
        _sound.Success();
        await Task.WhenAll(
            ScoreLabel.AnimateScale(1.15),
            UserTextLabel.AnimateScale(1.15));
    }

    private async Task OnWrongTextEntered(object args)
    {
        _sound.Fail();
        UserTextLabel.AnimateShake();
        await Task.CompletedTask;
    }

    private Task OnPenaltyApplied(object args)
    {
        TimeLeftLabel.AnimateShake(5);
        return Task.CompletedTask;
    }

    private bool _isExecuting = false;

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;

            _feedback.Perform();
            await _viewModel.RevealAnswer();
            await UserTextLabel.AnimateScale(1.15);
            await Task.Delay(750);
            await _viewModel.Next();
        }
        finally
        {
            _isExecuting = false;
        }
    }


    private async void OnHintClicked(object sender, EventArgs e)
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;
            _feedback.Perform();
            await _viewModel.Hint();
        }
        finally
        {
            _isExecuting = false;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;
            _feedback.Perform();
            var exit = await _viewModel.AskExitGame();
            if (exit)
                await _viewModel.DoGameOver();
        }
        finally
        {
            _isExecuting = false;
        }
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        await Task.CompletedTask;
    }

    private void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        _feedback.Vibrate();
        _viewModel.RemoveLastLetter();
    }
    private async void OnLetterClicked(object sender, EventArgs e)
    {
        if (_isExecuting)
            return;

        var button = sender as Button;
        if (button is null)
            return;

        if (button.Parent?.BindingContext is GameLetter letter)
        {
            _feedback.Perform();

            await _viewModel.ToggleLetter(letter);
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;
            _feedback.Perform();
            _viewModel.ClearUserWord();
        }
        finally
        {
            _isExecuting = false;
        }
    }
    private void OnClearLastLetter(object sender, EventArgs e)
    {
        _feedback.Perform();
        _viewModel.RemoveLastLetter();
    }

    public bool OnBackPressed()
    {
        OnBackClicked(null, null);
        return true;
    }
}