using WordFinder.Interfaces;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameSettingsPage : ContentPage, INavigationPage
{
    private readonly GameSettingsViewModel _viewModel;
    private readonly TouchFeedbackService _feedback;
    private readonly IBackNavigationHandler _backNavigationHandler;
    public GameSettingsPage(GameSettingsViewModel viewModel, TouchFeedbackService touchFeedbackService,
        IBackNavigationHandler backNavigationHandler)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _feedback = touchFeedbackService;
        _backNavigationHandler = backNavigationHandler;
        BindingContext = _viewModel;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
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

    private void OnBuyProClicked(object s, EventArgs e) 
    {
        _feedback.Perform();
        _viewModel.BuyPro();
    }

    protected async void OnSwipedRight(object sender, SwipedEventArgs e) => await GoHome();

    public bool OnBackPressed()
    {
        _ = GoHome();
        return true;
    }
}