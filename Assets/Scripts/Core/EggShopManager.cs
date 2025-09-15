using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EggShopManager : Singleton<EggShopManager>
{
    [Header("Dükkan Arayüz Elemanları")]
    [Tooltip("Yumurta dükkanını gösteren ana panel.")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Image eggImage;
    [SerializeField] private TextMeshProUGUI eggInfoText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button closeButton;

    private DragonEggData currentEggForSale;

    private void Start()
    {
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        closeButton.onClick.AddListener(HideShopPopup);
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Yumurta dükkanı panelini gösterir ve içini doldurur.
    /// </summary>
    public void ShowShopPopup(List<DragonEggData> eggsForSale)
    {
        if (eggsForSale == null || eggsForSale.Count == 0) return;

        currentEggForSale = eggsForSale[0];

        eggImage.sprite = currentEggForSale.eggSprite;
        eggInfoText.text = $"{currentEggForSale.eggName}\n<color=yellow>{currentEggForSale.cost:N0}</color>";

        // --- GÜNCELLENMİŞ KONTROL ---
        // Oyuncunun seviyesini artık yeni GameManager metodundan alıyoruz.
        bool canAfford = GameManager.Instance.TotalCoins >= currentEggForSale.cost;
        bool levelEnough = GameManager.Instance.GetCurrentDragonLevel() + 1 >= currentEggForSale.requiredPlayerLevel;

        purchaseButton.interactable = canAfford && levelEnough;
        shopPanel.SetActive(true);
    }

    private void HideShopPopup()
    {
        shopPanel.SetActive(false);
        currentEggForSale = null;
    }

    /// <summary>
    /// "Satın Al" butonuna tıklandığında çalışır ve yeni köy geçişini yönetir.
    /// </summary>
    private void OnPurchaseButtonClicked()
    {
        if (currentEggForSale == null) return;

        if (GameManager.Instance.TotalCoins >= currentEggForSale.cost)
        {
            // 1. Altını harca.
            GameManager.Instance.SpendGold(currentEggForSale.cost);

            Debug.Log($"{currentEggForSale.eggName} satın alındı!");

            // --- YENİ EKLENEN MANTIK ---
            // 2. Satın alınan yumurtanın bir köy kilidi açıp açmadığını kontrol et.
            if (currentEggForSale.villageToUnlock != null)
            {
                // 3. Eğer açıyorsa, GameManager'a yeni köyün kilidini açması ve oraya geçmesi için komut ver.
                GameManager.Instance.UnlockAndSwitchToVillage(currentEggForSale.villageToUnlock);
            }

            // 4. Paneli kapat.
            HideShopPopup();
        }
    }
}