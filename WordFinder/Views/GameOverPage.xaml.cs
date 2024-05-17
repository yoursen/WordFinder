using WordFinder.Interfaces;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameOverPage : ContentPage, INavigationPage
{
    private readonly GameOverViewModel _viewModel;
    private readonly TouchFeedbackService _feedback;
    private readonly IBackNavigationHandler _backNavigationHandle;
    public GameOverPage(GameOverViewModel viewModel, TouchFeedbackService feedback,
        IBackNavigationHandler backNavigationHandler)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _feedback = feedback;
        _backNavigationHandle = backNavigationHandler;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await GoHome();
    }

    private async void OnTryAgainClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await Navigation.PopAsync();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _backNavigationHandle.Page = this;
        base.OnNavigatedTo(args);
        await _viewModel.Refresh();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        _backNavigationHandle.Page = null;
        base.OnNavigatingFrom(args);
        _viewModel.OnNavigatingFrom();
    }
    private async Task GoHome() => await Navigation.PopToRootAsync();

    protected async void OnSwipedRight(object sender, SwipedEventArgs e) => await GoHome();

    private void OnBuyProClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        _viewModel.BuyPro();
    }

    public bool OnBackPressed()
    {
        _ = GoHome();
        return true;
    }
}