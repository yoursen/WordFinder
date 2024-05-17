using WordFinder.Interfaces;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameBestScorePage : ContentPage, INavigationPage
{
    private readonly GameBestScoreViewModel _viewModel;
    private readonly TouchFeedbackService _feedback;
    private readonly IBackNavigationHandler _backNavigationHandler;
    public GameBestScorePage(GameBestScoreViewModel viewModel, TouchFeedbackService feedback,
        IBackNavigationHandler backNavigationHandler)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _feedback = feedback;
        _backNavigationHandler = backNavigationHandler;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement)?.AnimateScale();
        await GoHome();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _backNavigationHandler.Page = this;
        base.OnNavigatedTo(args);
        await _viewModel.Refresh();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        _backNavigationHandler.Page = null;
        base.OnNavigatingFrom(args);
    }
    private async Task GoHome() => await Navigation.PopToRootAsync();

    protected async void OnSwipedRight(object sender, SwipedEventArgs e) => await GoHome();

    private void OnBuyProClicked(object sender, EventArgs e){
        _feedback.Perform();
        _viewModel.BuyPro();
    }

    public bool OnBackPressed()
    {
        _ = GoHome();
        return true;
    }
}