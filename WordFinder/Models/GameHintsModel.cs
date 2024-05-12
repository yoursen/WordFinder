using CommunityToolkit.Mvvm.ComponentModel;
using WordFinder.Services;

namespace WordFinder.Models;

/// <summary>
/// Represents the hint model.
/// 
/// For the free version of the application:
///     - Users have a limit of 20 hints per day.
///     - Each word allows up to 3 hints.
/// 
/// For the premium version of the application:
///     - Users have unlimited hints per day.
///     - Each word allows up to 5 hints.
///     
/// Note: At least 1 hint is always present.
/// </summary>
public partial class GameHintsModel : ObservableObject
{
    private const int HintsLimitPerDayForFree = 20;
    private const int WordLimitsInFreeApp = 3;
    private const int WordLimitsInProApp = 5;
    private readonly LicenseService _license;

    public GameHintsModel(LicenseService lisense)
    {
        _license = lisense;

        LastUsedDay = Preferences.Default.Get(nameof(LastUsedDay), 0);
        HintsLeftPerDay = Preferences.Default.Get(nameof(HintsLeftPerDay), HintsLimitPerDayForFree);
    }

    [ObservableProperty] private int _hintsLeft;

    private int HintsLimitPerWord => _license.IsFree ? WordLimitsInFreeApp : WordLimitsInProApp;

    private int _lastUsedDay;
    private int LastUsedDay
    {
        get => _lastUsedDay;
        set
        {
            if (_lastUsedDay != value)
            {
                _lastUsedDay = value;
                Preferences.Default.Set(nameof(LastUsedDay), value);
            }
        }
    }

    private int Today => DateTime.Now.Month * 100 + DateTime.Now.Day;

    private int _hintsLeftPerDay;
    public int HintsLeftPerDay
    {
        get
        {
            if (_license.IsPro)
                return int.MaxValue;

            if (LastUsedDay != Today)
            {
                _hintsLeftPerDay = HintsLimitPerDayForFree;
                LastUsedDay = Today;
            }

            return Math.Max(1, _hintsLeftPerDay); // always allow 1 hint
        }
        set
        {
            if (_license.IsPro)
                return;

            var newValue = Math.Max(1, value); // always allow 1 hint
            if (_hintsLeftPerDay != newValue)
            {
                _hintsLeftPerDay = newValue;
                Preferences.Default.Set(nameof(HintsLeftPerDay), newValue);
                OnPropertyChanged(nameof(HintsLeftPerDay));
            }
        }
    }

    public void UpdateHintsPerWord(int wordLength)
        => HintsLeft = Math.Min(HintsLeftPerDay, Math.Min(HintsLimitPerWord, wordLength - 2));

    public bool UseHint()
    {
        if (HintsLeft <= 0)
            return false;

        HintsLeft--;
        if (_license.IsFree)
        {
            HintsLeftPerDay--;
        }
        return true;
    }
}