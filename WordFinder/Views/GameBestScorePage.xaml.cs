using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameBestScorePage : ContentPage
{
    private GameBestScoreViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public GameBestScorePage(GameBestScoreViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _feedback = feedback;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        _feedback.DoFeedback();
        await (sender as VisualElement)?.AnimateScale();
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
    private async Task GoHome() => await Shell.Current.GoToAsync("///MainPage");

    protected async void OnSwipedRight(object sender, SwipedEventArgs e) => await GoHome();
}