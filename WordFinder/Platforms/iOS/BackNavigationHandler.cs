using WordFinder.Interfaces;

namespace WordFinder;

public class BackNavigationHandler : IBackNavigationHandler
{
    public INavigationPage Page { get; set; }
    public bool OnBackPressed() => false;
}