using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class GameModePopup : Popup
{
    private readonly GameModePopupViewModel _viewModel;
    private readonly TouchFeedbackService _feedback;
    private bool _isProgress;
    public GameModePopup(GameModePopupViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _feedback = feedback;
    }

    private async void On2minClicked(object sender, EventArgs e)
    {
        if (_isProgress)
            return;

        _isProgress = true;
        _feedback.Perform();
        _viewModel.StartGame(2);
        await CloseAsync();
        _isProgress = false;
    }

    private async void On5minClicked(object sender, EventArgs e)
    {
        if (_isProgress)
            return;

        _isProgress = true;
        _feedback.Perform();
        _viewModel.StartGame(5);
        await CloseAsync();
        _isProgress = false;
    }

    private async void On10minClicked(object sender, EventArgs e)
    {
        if (_isProgress)
            return;

        _isProgress = true;
        _feedback.Perform();
        _viewModel.StartGame(10);
        await CloseAsync();
        _isProgress = false;
    }

    private async void OnFreeplayClicked(object sender, EventArgs e)
    {
        if (_isProgress)
            return;

        _isProgress = true;
        _feedback.Perform();
        _viewModel.StartGame(-1);
        await CloseAsync();
        _isProgress = false;
    }
}