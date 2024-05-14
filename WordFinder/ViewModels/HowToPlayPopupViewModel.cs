using CommunityToolkit.Mvvm.ComponentModel;

namespace WordFinder.ViewModels;

public partial class HowToPlayPopupViewModel : ObservableObject
{
    [ObservableProperty] private int _pageNumber;

    public void Next()
    {
        PageNumber++;
    }
}