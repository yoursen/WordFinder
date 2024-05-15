using Android.App;
using Android.Content.PM;
using Android.OS;

namespace WordFinder;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        if(DeviceInfo.Current.Idiom != DeviceIdiom.Tablet){
            RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}
