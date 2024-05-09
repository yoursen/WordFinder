using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
using Google.UserMessagingPlatform;
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
		if (ConsentInformation.SharedInstance.ConsentStatus != Google.UserMessagingPlatform.ConsentStatus.Obtained)
		{
			var requestParameters = new RequestParameters();
			requestParameters.TagForUnderAgeOfConsent = false;
			requestParameters.DebugSettings = new DebugSettings() { };
			ConsentInformation.SharedInstance.RequestConsentInfoUpdateWithParameters(requestParameters, delegate (NSError? requestConsentError)
						{

							if (requestConsentError != null)
							{
								// mauiMTAdmob.MOnConsentInfoUpdateFailure(new MTEventArgs
								// {
								// 	ErrorCode = (int)requestConsentError.Code,
								// 	ErrorMessage = requestConsentError.Description,
								// 	ErrorDomain = requestConsentError.Domain
								// });
							}
							else
							{
								ConsentForm.LoadWithCompletionHandler(delegate (ConsentForm? f, NSError? e)
								{
									if (f != null)
									{
										UIViewController rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
										if (rootViewController != null)
										{
											f.PresentFromViewController(rootViewController, delegate (NSError? e)
											{
												if (e != null)
												{
													// mauiMTAdmob.MOnConsentFormLoadFailure(new MTEventArgs
													// {
													// 	ErrorCode = (int)e.Code,
													// 	ErrorMessage = e.Description,
													// 	ErrorDomain = e.Domain
													// });
												}
												else
												{
													InitAds();
													//mauiMTAdmob.MOnConsentFormLoadSuccess();
												}
											});
										}
									}
									if (e != null)
									{
										// mauiMTAdmob.MOnConsentFormLoadFailure(new MTEventArgs
										// {
										// 	ErrorCode = (int)e.Code,
										// 	ErrorMessage = e.Description,
										// 	ErrorDomain = e.Domain
										// });
									}
								});
								//mauiMTAdmob.MOnConsentInfoUpdateSuccess();
							}

						});
		}

		// if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
		// {
		// 	ATTrackingManager.RequestTrackingAuthorization((status) =>
		// 	{
		// 		// Handle the authorization status here
		// 		if (status == ATTrackingManagerAuthorizationStatus.Authorized)
		// 		{
		// 			// Tracking permission granted, you can now proceed with tracking
		// 		}
		// 		else
		// 		{
		// 			// Tracking permission not granted, handle accordingly
		// 		}
		// 	});
		// }
		// else
		// {

		// }
	}

	public void InitAds(bool isConsentObtained = false)
	{
		if (ConsentInformation.SharedInstance == null || ConsentInformation.SharedInstance.ConsentStatus != Google.UserMessagingPlatform.ConsentStatus.Obtained )
		{
			return;
		}
		MobileAds.SharedInstance.Start(delegate
		{

		});
	}

	private void ConsentFormCompletionHandler(ConsentForm consentForm, NSError error)
	{
		if (error != null)
		{

		}

		var infp = ConsentInformation.SharedInstance;

		var status = ConsentInformation.SharedInstance.ConsentStatus;
		if (status == ConsentStatus.Required)
		{
		}
	}
}
