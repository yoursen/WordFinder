using WordFinder.Interfaces;

namespace WordFinder;

public class Sound : ISound
{
    public bool IsPlayerReady => false;

    public void Fail()
    {
    }

    public void KeyboardClick()
    {
    }

    public void Success()
    {
    }
}