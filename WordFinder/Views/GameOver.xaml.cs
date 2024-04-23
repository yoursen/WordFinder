using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameOver : ContentPage
{
    private GameOverViewModel _viewModel;
    public GameOver(GameOverViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnMainMenuClicked(object sender, EventArgs e){
        await (sender as VisualElement)?.AnimateScale();
        await Shell.Current.GoToAsync("///MainPage");
    }

    private async void OnTryAgainClicked(object sender, EventArgs e){
        await (sender as VisualElement)?.AnimateScale();
        await Shell.Current.GoToAsync("..");
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
}