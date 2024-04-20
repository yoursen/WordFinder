using System.Windows.Input;

namespace WordFinder;

public class MainPageViewModel
{
    public ICommand PlayGameCommmand { get; }

    public MainPageViewModel()
    {
        PlayGameCommmand = new Command(PlayGameCommmandHandler);
    }

    private async void PlayGameCommmandHandler()
    {
        await Shell.Current.GoToAsync("./GamePage?GameDuration=2");
    }
}
