using System.Windows.Input;
using CommunityToolkit.Maui.Core;

namespace WordFinder.ViewModels;

public class MainPageViewModel
{
    public ICommand PlayGameCommmand { get; }
    private IPopupService _popupService;

    public MainPageViewModel(IPopupService popupService)
    {
        _popupService = popupService;
        PlayGameCommmand = new Command(PlayGameCommmandHandler);
    }

    private async void PlayGameCommmandHandler()
    {
        await _popupService.ShowPopupAsync<GameModePopupViewModel>();
        await Task.CompletedTask;
    }
}
