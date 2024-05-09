using CommunityToolkit.Maui.Converters;
using WordFinder.Interfaces;
using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Extra;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using System.Collections;

namespace WordFinder.Views;

public partial class GamePage : ContentPage
{
    private GamePageViewModel _viewModel;
    private AwaitableMessageService _ams;
    private TouchFeedbackService _feedback;
    private ISound _sound;
    public GamePage(GamePageViewModel viewModel, GameDatabase db, AwaitableMessageService ams,
        TouchFeedbackService touchFeedbackService, ISound sound)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        LastShownInterstial = DateTime.Now;

        _ams = ams;
        _feedback = touchFeedbackService;
        _sound = sound;
    }

    private void OnInterstitialLoaded(object sender, EventArgs e)
    {
        IsInterstitialLoaded = true;
    }
    private TimeSpan InterstitialThreshold { get; set; } = TimeSpan.FromMinutes(2);

    private void ShowInterstitialAds()
    {
        if (!IsInterstitialLoaded)
            return;

        if (DateTime.Now - LastShownInterstial > InterstitialThreshold)
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        }
    }

    private bool IsInterstitialLoaded { get; set; }
    private DateTime LastShownInterstial { get; set; }

    private void LoadInterstitial() => CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-4999851819408381/6366330626");
    private void OnInterstitialClosed(object sender, EventArgs e)
    {
        _viewModel.ResumeGame();
        LoadInterstitial();
        LastShownInterstial = DateTime.Now;
    }

    private void OnInterstitialOpened(object sender, EventArgs e)
    {
        _viewModel.SuspendGame();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.OnNavigatedTo();

        _ams.Register("WrongTextEntered", OnWrongTextEntered);
        _ams.Register("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Register("PenaltyApplied", OnPenaltyApplied);

        myAds.LoadAd();
        LoadInterstitial();

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += OnInterstitialLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialClosed += OnInterstitialClosed;
        CrossMauiMTAdmob.Current.OnInterstitialOpened += OnInterstitialOpened;
    }


    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        await _viewModel.OnNavigatingFrom();
        CrossMauiMTAdmob.Current.OnInterstitialLoaded -= OnInterstitialLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialClosed -= OnInterstitialClosed;
        CrossMauiMTAdmob.Current.OnInterstitialOpened -= OnInterstitialOpened;
        _ams.Unregister("WrongTextEntered", OnWrongTextEntered);
        _ams.Unregister("CorrectTextEntered", OnCorrectTextEntered);
        _ams.Unregister("PenaltyApplied", OnPenaltyApplied);
    }

    private async Task OnCorrectTextEntered(object args)
    {
        _sound.Success();
        await Task.WhenAll(
            ScoreLabel.AnimateScale(1.15),
            UserTextLabel.AnimateScale(1.15));
        ShowInterstitialAds();
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

            ShowInterstitialAds();
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
        _feedback.Perform();
        var exit = await _viewModel.AskExitGame();
        if (exit)
            await _viewModel.DoGameOver();
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        await Task.CompletedTask;
    }

    private void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        // do nothing
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}