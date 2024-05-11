using System.Windows.Input;
using CommunityToolkit.Maui.Core;

namespace WordFinder.ViewModels;

public class MainPageViewModel
{
    public ICommand PlayGameCommmand { get; }
    public ICommand GameSettingsCommmand { get; }
    public ICommand BestScoreCommmand { get; }
    private readonly IPopupService _popupService;

    public MainPageViewModel(IPopupService popupService)
    {
        _popupService = popupService;
        PlayGameCommmand = new Command(PlayGameCommmandHandler);
        GameSettingsCommmand = new Command(GameSettingsCommmandHandler);
        BestScoreCommmand = new Command(BestScoreCommmandHandler);
    }

    private async void PlayGameCommmandHandler()
    {
        await _popupService.ShowPopupAsync<GameModePopupViewModel>();
        await Task.CompletedTask;
    }

    private async void GameSettingsCommmandHandler()
    {
        await Shell.Current.GoToAsync("GameSettings");
    }

    private async void BestScoreCommmandHandler()
    {
        await Shell.Current.GoToAsync("GameBestScore");
    }
}
