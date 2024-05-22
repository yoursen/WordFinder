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
        bool pendingPurchase = false;
        try
        {
            billing = CrossInAppBilling.Current;
            var connected = billing.IsConnected ? true : await billing.ConnectAsync();
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
#if __ANDROID__
                await CrossInAppBilling.Current.FinalizePurchaseAsync(purchase.TransactionIdentifier);
#endif
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
            if (!pendingPurchase)
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
            var connected = billing.IsConnected ? true : await billing.ConnectAsync();
            if (!connected)
            {
                await ShowToast("Cannot connect to store.");
                return false;
            }

            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);

            //check for null just in case
            var purchase = purchases?.FirstOrDefault(p => p.ProductId == WordFinderPremiumProductId);
            if (purchase != null)
            {
#if __ANDROID__
                if(purchase.IsAcknowledged != true)
                    await CrossInAppBilling.Current.FinalizePurchaseAsync(purchase.TransactionIdentifier);
#endif
                IsPro = true;
                await ShowToast("Purchase succesfully restored.");
            }
            else
            {
                await ShowToast("Cannot find the purchase.");
            }
        }
        catch (InAppBillingPurchaseException ex)
        {
            await ShowToast(ex.Message);
        }
        catch (Exception ex)
        {
            await ShowToast(ex.Message);
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