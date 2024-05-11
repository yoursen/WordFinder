using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameOverPage : ContentPage
{
    private GameOverViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public GameOverPage(GameOverViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _feedback = feedback;
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
        base.OnNavigatedTo(args);
        await _viewModel.Refresh();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
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
}