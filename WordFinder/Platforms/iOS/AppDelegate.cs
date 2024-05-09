using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
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
		RequestTrackingPermission();
	}

	private void InitializationHandler(InitializationStatus status)
	{

	}

	public void RequestTrackingPermission()
	{
		if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
		{
			ATTrackingManager.RequestTrackingAuthorization((status) =>
			{
				// Handle the authorization status here
				if (status == ATTrackingManagerAuthorizationStatus.Authorized)
				{
					// Tracking permission granted, you can now proceed with tracking
				}
				else
				{
					// Tracking permission not granted, handle accordingly
				}
			});
		}
		else
		{

		}
	}
}
