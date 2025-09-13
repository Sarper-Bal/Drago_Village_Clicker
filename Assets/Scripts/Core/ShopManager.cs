using UnityEngine;
using System.Collections.Generic; // List kullanmak için bu kütüphane gerekli.

public class ShopManager : Singleton<ShopManager>
{
    [Header("Mağaza Ayarları")]
    [Tooltip("Mağazada gösterilecek olan ilk seviye geliştirmelerin listesi.")]
    [SerializeField] private List<UpgradeData> initialUpgrades;

    // TODO: Oyuncunun hangi geliştirmeleri satın aldığını ve hangi seviyede olduğunu tutacak bir kayıt sistemi eklenecek.
    // Örneğin: private Dictionary<string, int> purchasedUpgradeLevels = new Dictionary<string, int>();

    // TODO: Mağaza UI panelini açıp kapatacak fonksiyonlar eklenecek.
    // public void OpenShopPanel() { ... }
    // public void CloseShopPanel() { ... }

    /// <summary>
    /// Bir geliştirme satın alındığında çalışacak olan ana mantık.
    /// UI'daki buton tarafından çağrılacak.
    /// </summary>
    /// <param name="upgradeToPurchase">Satın alınmak istenen geliştirmenin verisi.</param>
    public void PurchaseUpgrade(UpgradeData upgradeToPurchase)
    {
        // 1. Oyuncunun yeterli altını var mı diye kontrol et.
        if (GameManager.Instance.TotalCoins < upgradeToPurchase.cost)
        {
            Debug.Log("Yeterli altın yok!");
            // TODO: Oyuncuya "Yeterli Altın Yok" uyarısı göster.
            return;
        }

        // 2. Altını harca.
        // TODO: GameManager'a altını düşmesi için bir komut gönderecek bir metot yazılmalı.
        // GameManager.Instance.SpendGold(upgradeToPurchase.cost);

        // 3. Pasif geliri artır.
        PassiveIncomeManager.Instance.AddGoldPerSecond(upgradeToPurchase.goldPerSecondBonus);

        // 4. Bina animasyonunu tetikle.
        if (!string.IsNullOrEmpty(upgradeToPurchase.targetBuildingID))
        {
            // TODO: VillageManager'a animasyon oynatması için haber ver.
            // VillageManager.Instance.PlayAnimationOnBuilding(upgradeToPurchase.targetBuildingID);
        }

        // 5. Satın alınan geliştirmeyi kaydet ve bir sonraki seviyeyi mağazada göstermek için hazırla.
        // TODO: Geliştirme kayıt sistemi ve UI güncelleme mantığı eklenecek.

        Debug.Log($"{upgradeToPurchase.upgradeName} satın alındı!");
    }
}