using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class ExitGamePopup : Popup
{
    private ExitGamePopupViewModel _viewModel;
    private TouchFeedbackService _feedback;
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
        await (sender as VisualElement).AnimateScale();
        await CloseAsync(true);
    }

    private async void NoClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await (sender as VisualElement).AnimateScale();
        await CloseAsync(false);
    }
}