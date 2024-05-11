using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class NoWordsPopup : Popup
{
    private readonly NoWordsPopupViewModel _viewModel;
    private TouchFeedbackService _feedback;
    public NoWordsPopup(NoWordsPopupViewModel viewModel, TouchFeedbackService feedback)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _feedback = feedback;

        Opened += OnOpened;
    }

    private async void OnOpened(object sender, PopupOpenedEventArgs e)
    {
        await _viewModel.Refresh();
    }

    private async void OKClicked(object sender, EventArgs e)
    {
        _feedback.Perform();
        await CloseAsync(true);
    }
}