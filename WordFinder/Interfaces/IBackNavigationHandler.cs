namespace WordFinder.Interfaces;

public interface IBackNavigationHandler
{
    INavigationPage Page { get; set; }
    bool OnBackPressed();
}
