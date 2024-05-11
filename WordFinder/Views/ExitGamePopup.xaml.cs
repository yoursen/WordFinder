using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class ExitGamePopup : Popup
{
    private readonly ExitGamePopupViewModel _viewModel;
    private readonly TouchFeedbackService _feedback;
    public ExitGamePopup(ExitGamePopupViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _feedback = feedback;
    }

    private async void YesClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await CloseAsync(true);
    }

    private async void NoClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await CloseAsync(false);
    }
}