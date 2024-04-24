using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WordFinder.Messages;

public class AppResumedMessage : ValueChangedMessage<bool>
{
    public AppResumedMessage() : base(true) { }
}