using System.Windows.Input;

namespace WordFinder;

public class MainPageViewModel
{
    private WordsDatabase _database;

    public ICommand PlayGameCommmand { get; }

    public MainPageViewModel(WordsDatabase database)
    {
        _database = database;
        PlayGameCommmand = new Command(PlayGameCommmandHandler);
    }

    private async void PlayGameCommmandHandler(){
        await Shell.Current.GoToAsync("./GamePage");
    }
}
