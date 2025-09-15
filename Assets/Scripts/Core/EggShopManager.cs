using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EggShopManager : Singleton<EggShopManager>
{
    [Header("Dükkan Arayüz Elemanları")]
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

    public void ShowShopPopup(List<DragonEggData> eggsForSale)
    {
        if (eggsForSale == null || eggsForSale.Count == 0) return;

        currentEggForSale = eggsForSale[0];

        eggImage.sprite = currentEggForSale.eggSprite;
        eggInfoText.text = $"{currentEggForSale.eggName}\n<color=yellow>{currentEggForSale.cost:N0}</color>";

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

    private void OnPurchaseButtonClicked()
    {
        if (currentEggForSale == null) return;

        if (GameManager.Instance.TotalCoins >= currentEggForSale.cost)
        {
            GameManager.Instance.SpendGold(currentEggForSale.cost);

            if (currentEggForSale.villageToUnlock != null)
            {
                GameManager.Instance.UnlockAndSwitchToVillage(currentEggForSale.villageToUnlock);
            }

            HideShopPopup();
        }
    }
}