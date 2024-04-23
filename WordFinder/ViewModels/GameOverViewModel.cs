using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Models;

namespace WordFinder.ViewModels;

public partial class GameOverViewModel : ObservableObject
{
    private GameModel _gameModel;
    private GameDatabase _db;
    public GameOverViewModel(GameModel gameModel, GameDatabase db)
    {
        _gameModel = gameModel;
        _db = db;
    }

    [ObservableProperty] private int _score;
    [ObservableProperty] private int _bestScore;
    [ObservableProperty] private bool _isRecord;
    [ObservableProperty] private int _gameDuration;

    public async Task Refresh()
    {
        var gameScore = await _db.GetLastGameScore();
        var bestScore = await _db.GetBestGameScore(_gameModel.GameDuration);

        GameDuration = gameScore.GameDuration;
        Score = gameScore.Score;
        BestScore = bestScore?.Score ?? Score;
        IsRecord = Score > BestScore;

        await Task.CompletedTask;
        return;
    }

    public void OnNavigatingFrom(){
        GameDuration = 0;
        BestScore = 0;
        Score = 0;
        IsRecord = false;
    }
}