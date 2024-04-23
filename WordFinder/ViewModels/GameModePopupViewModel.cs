namespace WordFinder.ViewModels;

public class GameModePopupViewModel : BindableObject
{
    public async void StartGame(int gameDuration)
    {
        await Shell.Current.GoToAsync($"./GamePage?GameDuration={gameDuration}");
    }
}