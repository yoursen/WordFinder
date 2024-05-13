using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using Plugin.InAppBilling;

namespace WordFinder.Services;

public class LicenseService
{
    private const string WordFinderPremiumProductId = "com.seniuk.wordfinder.premium";

    public bool IsPro { get; private set; }
    public bool IsFree => !IsPro;

    public async Task<bool> BuyPro()
    {
        if (!CrossInAppBilling.IsSupported)
            return false;

        IInAppBilling billing = null;
        try
        {
            billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            if (!connected)
            {
                await ShowToast("Cannot connect");
                return false;
            }
            var pruducts = await billing.GetProductInfoAsync(ItemType.InAppPurchase);
            var purchase = await billing.PurchaseAsync(WordFinderPremiumProductId, ItemType.InAppPurchase);

            //possibility that a null came through.
            if (purchase == null)
            {
                IsPro = false;
            }
            else if (purchase.State == PurchaseState.Purchased)
            {
                // only need to finalize if on Android unless you turn off auto finalize on iOS
                // var ack = await CrossInAppBilling.Current.FinalizePurchaseAsync(purchase.TransactionIdentifier);
                // handle if acknowledge was successful or not
                IsPro = true;
            }
            else if (purchase.State == PurchaseState.Restored)
            {
                IsPro = true;
            }

            await ShowToast("IsPro:" + IsPro + " State:" + purchase?.State ?? "null");
        }
        catch (InAppBillingPurchaseException purchaseEx)
        {
            Debug.WriteLine("Error: " + purchaseEx);
            await ShowToast(purchaseEx.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex);
            await ShowToast(ex.Message);
        }
        finally
        {
            await billing?.DisconnectAsync();
        }
        return await Task.FromResult(IsPro);
    }

    public async Task<bool> RestoreProLicense()
    {
        var billing = CrossInAppBilling.Current;
        try
        {
            var connected = await billing.ConnectAsync();

            if (!connected)
            {
                //Couldn't connect
                await ShowToast("Cannot connect");
                return false;
            }
            var list = await billing.GetProductInfoAsync(ItemType.InAppPurchase);
            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);

            //check for null just in case
            if (purchases?.Any(p => p.ProductId == WordFinderPremiumProductId) ?? false)
            {
                // purchase restored
                // if on Android may be good to check if these purchases need to be acknowledge
                IsPro = true;
            }
            else
            {
            }
            await ShowToast("IsPro:" + IsPro);
        }
        catch (InAppBillingPurchaseException purchaseEx)
        {
            Debug.WriteLine("Error: " + purchaseEx);
            await ShowToast(purchaseEx.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex);
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