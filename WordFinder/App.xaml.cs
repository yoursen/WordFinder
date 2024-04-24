using CommunityToolkit.Mvvm.Messaging;
using WordFinder.Messages;

namespace WordFinder;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}

	protected override void OnSleep()
	{
		base.OnSleep();
		WeakReferenceMessenger.Default.Send(new AppSuspendedMessage());
	}

	protected override void OnResume()
	{
		base.OnResume();
		WeakReferenceMessenger.Default.Send(new AppResumedMessage());
	}
}
