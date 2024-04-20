using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Models;

namespace WordFinder.ViewModels;

public partial class GameOverViewModel : ObservableObject
{
    private GameModel _gameModel;
    public GameOverViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
    }

    [ObservableProperty] private int _score;
    [ObservableProperty] private int _bestScore;
    [ObservableProperty] private bool _isRecord;

    public async Task Refresh()
    {
        Score = _gameModel.Score;
        BestScore = Random.Shared.Next(0, 5);
        IsRecord = Score > BestScore;

        await Task.CompletedTask;
        return;
    }
}