namespace WordFinder.Services;

public class LicenseService
{
    public bool IsPro { get; private set; }
    public bool IsFree => !IsPro;

    public async Task BuyPro()
    {
        await Task.Delay(2000);
        IsPro = true;
    }
}