using CommunityToolkit.Maui.Views;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameModePopup : Popup
{
    private GameModePopupViewModel _viewModel;
    public GameModePopup(GameModePopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private async void On2minClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(2);
    }

    private async void On5minClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(5);
    }

    private async void On10minClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(10);
    }
}