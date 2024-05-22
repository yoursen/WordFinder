using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using Plugin.InAppBilling;

namespace WordFinder.Services;

public class LicenseService
{
    private const string WordFinderPremiumProductId = "com.seniuk.wordfinder.premium";

    public LicenseService()
    {
        _isPro = Preferences.Default.Get(nameof(IsPro), false);
    }

    private bool _isPro;
    public bool IsPro
    {
        get => _isPro;
        private set
        {
            if (_isPro != value && value)
            {
                _isPro = value;
                Preferences.Default.Set(nameof(IsPro), value);
            }
        }
    }
    public bool IsFree => !IsPro;

    public async Task<bool> BuyPro()
    {
        if (IsPro)
            return true;

        if (!CrossInAppBilling.IsSupported)
            return false;

        IInAppBilling billing = null;
        try
        {
            billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            if (!connected)
            {
                await ShowToast("Cannot connect to store.");
                return false;
            }

            var purchase = await billing.PurchaseAsync(WordFinderPremiumProductId, ItemType.InAppPurchase);
            if (purchase == null)
            {
            }
            else if (purchase.State == PurchaseState.Purchased)
            {
                IsPro = true;
            }
            else if (purchase.State == PurchaseState.Restored)
            {
                IsPro = true;
            }
        }
        catch (InAppBillingPurchaseException)
        {
        }
        catch (Exception)
        {
        }
        finally
        {
            await billing?.DisconnectAsync();
        }
        return await Task.FromResult(IsPro);
    }

    public async Task<bool> RestoreProLicense()
    {
        if (IsPro)
            return true;

        var billing = CrossInAppBilling.Current;
        try
        {
            var connected = await billing.ConnectAsync();
            if (!connected)
            {
                await ShowToast("Cannot connect to store.");
                return false;
            }

            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);

            //check for null just in case
            if (purchases?.Any(p => p.ProductId == WordFinderPremiumProductId) ?? false)
            {
                IsPro = true;
                await ShowToast("Purchase succesfully restored.");
            }
            else
            {
                await ShowToast("Cannot find the purchase.");
            }
        }
        catch (InAppBillingPurchaseException)
        {
        }
        catch (Exception)
        {
        }
        finally
        {
            await billing.DisconnectAsync();
        }

        return IsPro;
    }

    private async Task ShowToast(string msg)
    {
        var toast = Toast.Make(msg);
        await toast.Show();
    }

}