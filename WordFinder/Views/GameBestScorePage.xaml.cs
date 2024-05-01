using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameBestScorePage : ContentPage
{
    private GameBestScoreViewModel _viewModel;
    public GameBestScorePage(GameBestScoreViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e){
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