using System.ComponentModel;

namespace WordFinder;

public class GamePageViewModel : BindableObject
{
    private GameModel _gameModel;

    public GamePageViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
    }

    public GameLetter[] Letters => _gameModel.Letters;
    public GameWord GuessWord => _gameModel.GuessWord;
    public string UserWord => _gameModel.UserWord;

    public async Task Next() => await _gameModel.Next();
    public void Reset() => _gameModel.Reset();
    public void ToggleButton(CharButtonView ch) => _gameModel.ToggleButton(ch);
    private void GameModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        => OnPropertyChanged(e.PropertyName);

    public async Task OnNavigatedTo()
    {
        _gameModel.PropertyChanged += GameModelPropertyChanged;
        await _gameModel.Next();
    }

    public async Task OnNavigatedFrom()
    {
        _gameModel.PropertyChanged -= GameModelPropertyChanged;
        await _gameModel.Reset();
    }
}