using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class HowToPlayPopup : Popup
{
    private readonly HowToPlayPopupViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public HowToPlayPopup(HowToPlayPopupViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _feedback = feedback;
    }

    private void NextClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        _viewModel.Next();
    }

    private async void CloseClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await CloseAsync(true);
    }
}