using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
using Google.UserMessagingPlatform;
using UIKit;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Extra;

namespace WordFinder;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication app, NSDictionary options)
	{
		InitAds();
		return base.FinishedLaunching(app, options);
	}

	public override void OnActivated(UIApplication application)
	{
		base.OnActivated(application);
		RequestTrackingPermission();
	}

	public void RequestTrackingPermission()
	{
		if (ConsentInformation.SharedInstance.ConsentStatus != Google.UserMessagingPlatform.ConsentStatus.Obtained)
		{
			var requestParameters = new RequestParameters();
			requestParameters.TagForUnderAgeOfConsent = false;
			requestParameters.DebugSettings = new DebugSettings() { };
			ConsentInformation.SharedInstance.RequestConsentInfoUpdateWithParameters(requestParameters, delegate (NSError requestConsentError)
						{

							if (requestConsentError != null)
							{
							}
							else
							{
								ConsentForm.LoadWithCompletionHandler(delegate (ConsentForm f, NSError e)
								{
									if (f != null)
									{
#pragma warning disable CA1422 // Validate platform compatibility
                                        UIViewController rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
#pragma warning restore CA1422 // Validate platform compatibility
                                        if (rootViewController != null)
										{
											f.PresentFromViewController(rootViewController, delegate (NSError e)
											{
												if (e != null)
												{
												}
												else
												{
													InitAds();
												}
											});
										}
									}
									if (e != null)
									{
									}
								});
							}
						});
		}
	}

	public void InitAds(bool isConsentObtained = false)
	{
		if (Google.MobileAds.MobileAds.SharedInstance == null)
			return;

		Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);
	}
}
