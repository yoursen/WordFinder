using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameSettingsPage : ContentPage
{
    private GameSettingsViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public GameSettingsPage(GameSettingsViewModel viewModel, TouchFeedbackService touchFeedbackService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _feedback = touchFeedbackService;
        BindingContext = _viewModel;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await GoHome();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Refresh();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
    }
    private async Task GoHome() => await Navigation.PopToRootAsync();

    private void OnBuyProClicked(object s, EventArgs e) 
    {
        _feedback.Perform();
        _viewModel.BuyPro();
    }

    protected async void OnSwipedRight(object sender, SwipedEventArgs e) => await GoHome();
}