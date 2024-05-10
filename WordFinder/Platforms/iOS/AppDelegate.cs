using Foundation;
using UIKit;

namespace WordFinder;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication app, NSDictionary options)
	{
		return base.FinishedLaunching(app, options);
	}

	public override void OnActivated(UIApplication application)
	{
		base.OnActivated(application);
	}
}
