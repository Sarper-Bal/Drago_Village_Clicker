using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// --- DÜZELTME BURADA ---
// Script'i MonoBehaviour yerine Singleton<EggShopManager>'dan türetiyoruz.
// Bu, ona "Instance" özelliğini kazandırır.
public class EggShopManager : Singleton<EggShopManager>
{
    [Header("Dükkan Arayüz Elemanları")]
    [Tooltip("Yumurta dükkanını gösteren ana panel.")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Image eggImage;
    [SerializeField] private TextMeshProUGUI eggInfoText; // İsim ve fiyat için
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button closeButton;

    // O anda dükkanda gösterilen yumurtanın verisini saklamak için.
    private DragonEggData currentEggForSale;

    // Singleton'dan türediği için artık Awake metodunu override etmemiz gerekmiyor,
    // ana Singleton sınıfı bunu bizim için hallediyor.
    // Ancak oyun başında panelin kapalı olmasını sağlamak için Start metodunu kullanabiliriz.
    private void Start()
    {
        // Butonların tıklama olaylarını ilgili fonksiyonlara bağla.
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        closeButton.onClick.AddListener(HideShopPopup);

        // Oyun başında panelin kapalı olduğundan emin ol.
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Yumurta dükkanı panelini gösterir ve içini doldurur.
    /// EggShopBuilding tarafından çağrılır.
    /// </summary>
    public void ShowShopPopup(List<DragonEggData> eggsForSale)
    {
        // Eğer satılacak yumurta yoksa paneli açma.
        if (eggsForSale == null || eggsForSale.Count == 0) return;

        // Sade bir dükkan için şimdilik sadece listedeki İLK yumurtayı gösteriyoruz.
        currentEggForSale = eggsForSale[0];

        // Paneli doldur.
        eggImage.sprite = currentEggForSale.eggSprite;
        eggInfoText.text = $"{currentEggForSale.eggName}\n<color=yellow>{currentEggForSale.cost:N0}</color>"; // İsim ve fiyat

        // Oyuncunun seviyesi ve altını yeterli mi diye kontrol et.
        bool canAfford = GameManager.Instance.TotalCoins >= currentEggForSale.cost;
        bool levelEnough = GameManager.Instance.CurrentLevelIndex + 1 >= currentEggForSale.requiredPlayerLevel;

        // Eğer her iki koşul da sağlanıyorsa butonu aktif et.
        purchaseButton.interactable = canAfford && levelEnough;

        // Paneli görünür yap.
        shopPanel.SetActive(true);
    }

    /// <summary>
    /// Dükkan panelini gizler.
    /// </summary>
    private void HideShopPopup()
    {
        shopPanel.SetActive(false);
        currentEggForSale = null; // Saklanan yumurta referansını temizle.
    }

    /// <summary>
    /// "Satın Al" butonuna tıklandığında çalışır.
    /// </summary>
    private void OnPurchaseButtonClicked()
    {
        if (currentEggForSale == null) return;

        // Son bir kontrol daha yap.
        if (GameManager.Instance.TotalCoins >= currentEggForSale.cost)
        {
            // 1. Altını harca.
            GameManager.Instance.SpendGold(currentEggForSale.cost);

            Debug.Log($"{currentEggForSale.eggName} satın alındı!");

            // TODO: Satın alınan yumurtanın oyuncuya verilmesiyle ilgili mantık buraya eklenecek.
            // Örneğin, bu yumurtayı listeden kaldırabilir veya "satın alındı" olarak işaretleyebiliriz.

            // 2. Paneli kapat.
            HideShopPopup();
        }
    }
}