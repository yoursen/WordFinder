using WordFinder.Resources.Strings;
using WordFinder.Services;

namespace WordFinder.ViewModels;

public class NoWordsPopupViewModel : BindableObject
{
    private readonly LicenseService _license;

    public NoWordsPopupViewModel(LicenseService license)
    {
        _license = license;
    }

    public bool IsFree => _license.IsFree;

    public string Message
    {
        get
        {
            if (IsFree)
                return AppResources.NoWordsFree;
            else
                return AppResources.NoWordsPremium;
        }
    }

    public async Task Refresh()
    {
        OnPropertyChanged(nameof(Message));
        await Task.CompletedTask;
    }
}