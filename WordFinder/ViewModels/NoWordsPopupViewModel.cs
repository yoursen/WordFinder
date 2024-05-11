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
                return "You've guessed all the words in the free version of the application. The word list will now be reset. Consider upgrading to the premium version to unlock more words and enjoy an enhanced experience. ";
            else
                return "You've successfully guessed all the words in the premium version of the application. We appreciate your choice in selecting this game. Please note that the word list will now be reset to keep the challenge fresh for you. Thank you for being part of our gaming community!";
        }
    }

    public async Task Refresh()
    {
        OnPropertyChanged(nameof(Message));
        await Task.CompletedTask;
    }
}