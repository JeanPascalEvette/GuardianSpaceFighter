using UnityEngine;
using UnityEngine.Purchasing;

//This function is used for In App Purchases and is based on Unity's tutorial
public class Purchaser : IStoreListener
{

    private static IStoreController mController;

    public Purchaser()
    {

    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
        builder.AddProduct("GuardianSpaceFighterPremiumCurrency", ProductType.Consumable, new IDs
        {
            {"GuardianSpaceFighterPremiumCurrency", WindowsStore.Name}
        });

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        mController = controller;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }


    public void BuyCurrency()
    {
        mController.InitiatePurchase("GuardianSpaceFighterPremiumCurrency");
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        UserData.BuyRealMoneyPoints();
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
    }
}