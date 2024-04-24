using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WordFinder.Messages;

public class AppSuspendedMessage : ValueChangedMessage<bool>
{
    public AppSuspendedMessage() : base(true) { }
}