using UIKit;
using WordFinder.Interfaces;

namespace WordFinder;

public class Sound : ISound
{
    public void KeyboardClick()
    {
        UIDevice.CurrentDevice.PlayInputClick();
    }
}