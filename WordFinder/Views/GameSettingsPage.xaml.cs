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

    private async void OnMainMenuClicked(object sender, EventArgs e){
        _feedback.DoFeedback();
        await (sender as VisualElement)?.AnimateScale();
        await Shell.Current.GoToAsync("///MainPage");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
    }
}