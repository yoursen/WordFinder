namespace WordFinder.Interfaces;
public interface ISound
{
    bool IsPlayerReady { get; }
    void KeyboardClick();
    void Success();
    void Fail();
}