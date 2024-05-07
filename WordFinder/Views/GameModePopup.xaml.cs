using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameModePopup : Popup
{
    private GameModePopupViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public GameModePopup(GameModePopupViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _feedback = feedback;
    }

    private async void On2minClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(2);
    }

    private async void On5minClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(5);
    }

    private async void On10minClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(10);
    }

    private async void OnFreeplayClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement).AnimateScale();
        await CloseAsync();
        _viewModel.StartGame(-1);
    }
}