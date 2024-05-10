using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Models;

namespace WordFinder.ViewModels;

public partial class GameBestScoreViewModel : ObservableObject
{
    private readonly GameDatabase _db;

    [ObservableProperty] private int _score2min;
    [ObservableProperty] private int _score5min;
    [ObservableProperty] private int _score10min;
    [ObservableProperty] private int _scoreFreeplay;
    [ObservableProperty] private int _totalWords;
    [ObservableProperty] private int _totalWordsAnswered;

    public GameBestScoreViewModel(GameDatabase db)
    {
        _db = db;
    }

    public async Task Refresh()
    {
        Score2min = (await _db.GetBestGameScore(2))?.Score ?? 0;
        Score5min = (await _db.GetBestGameScore(5))?.Score ?? 0;
        Score10min = (await _db.GetBestGameScore(10))?.Score ?? 0;
        ScoreFreeplay = (await _db.GetBestGameScore(-1))?.Score ?? 0;
        TotalWords = await _db.CountWords();
        TotalWordsAnswered = await _db.CountWordsAnswered();
    }
}