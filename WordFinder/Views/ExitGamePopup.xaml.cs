using CommunityToolkit.Maui.Views;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class ExitGamePopup : Popup
{
    private ExitGamePopupViewModel _viewModel;
    public ExitGamePopup(ExitGamePopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private async void YesClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await CloseAsync(true);
    }

    private async void NoClicked(object sender, EventArgs e)
    {
        await (sender as VisualElement).AnimateScale();
        await CloseAsync(false);
    }
}