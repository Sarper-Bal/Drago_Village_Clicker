using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    [Header("Mağaza Ayarları")]
    [SerializeField] private List<UpgradeData> initialUpgrades;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject upgradeItemPrefab;

    private Dictionary<string, UpgradeData> currentUpgrades = new Dictionary<string, UpgradeData>();

    private void Start()
    {
        foreach (var upgrade in initialUpgrades)
        {
            if (!currentUpgrades.ContainsKey(upgrade.upgradeID))
            {
                currentUpgrades.Add(upgrade.upgradeID, upgrade);
            }
        }
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void OpenShopPanel()
    {
        if (shopPanel == null) return;
        shopPanel.SetActive(true);
        RefreshShop();
    }

    public void CloseShopPanel()
    {
        if (shopPanel == null) return;
        shopPanel.SetActive(false);
    }

    private void RefreshShop()
    {
        if (contentParent == null || upgradeItemPrefab == null)
        {
            Debug.LogError("ShopManager'da Content Parent veya Upgrade Item Prefab'ı atanmamış!");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in currentUpgrades.Values)
        {
            if (GameManager.Instance.CurrentLevelIndex + 1 >= upgrade.minPlayerLevel)
            {
                GameObject itemGO = Instantiate(upgradeItemPrefab, contentParent);

                // İkonu ayarla
                Image iconImage = itemGO.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null) iconImage.sprite = upgrade.icon;
                else Debug.LogWarning($"Prefab'da 'Icon' objesi veya üzerinde Image bileşeni bulunamadı!");

                // İsim metnini ayarla
                TextMeshProUGUI nameText = itemGO.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null) nameText.text = upgrade.upgradeName;
                else Debug.LogWarning($"Prefab'da 'NameText' objesi veya üzerinde TextMeshProUGUI bileşeni bulunamadı!");

                // Açıklama metnini ayarla
                TextMeshProUGUI descText = itemGO.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
                if (descText != null) descText.text = upgrade.description;
                else Debug.LogWarning($"Prefab'da 'DescriptionText' objesi veya üzerinde TextMeshProUGUI bileşeni bulunamadı!");

                // Buton ve maliyet metnini ayarla
                Button purchaseButton = itemGO.transform.Find("CostButton")?.GetComponent<Button>();
                TextMeshProUGUI costText = purchaseButton?.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();

                if (purchaseButton != null && costText != null)
                {
                    costText.text = upgrade.cost.ToString();
                    purchaseButton.onClick.AddListener(() => PurchaseUpgrade(upgrade));
                    purchaseButton.interactable = (GameManager.Instance.TotalCoins >= upgrade.cost);
                }
                else
                {
                    Debug.LogWarning($"Prefab'da 'CostButton' veya içinde 'CostText' objesi bulunamadı!");
                }
            }
        }
    }

    public void PurchaseUpgrade(UpgradeData upgradeToPurchase)
    {
        if (GameManager.Instance.TotalCoins < upgradeToPurchase.cost) return;

        GameManager.Instance.SpendGold(upgradeToPurchase.cost);
        PassiveIncomeManager.Instance.AddGoldPerSecond(upgradeToPurchase.goldPerSecondBonus);

        if (!string.IsNullOrEmpty(upgradeToPurchase.targetBuildingID))
        {
            VillageManager.Instance.PlayAnimationOnBuilding(upgradeToPurchase.targetBuildingID);
        }

        if (upgradeToPurchase.nextUpgrade != null)
        {
            currentUpgrades[upgradeToPurchase.upgradeID] = upgradeToPurchase.nextUpgrade;
        }
        else
        {
            currentUpgrades.Remove(upgradeToPurchase.upgradeID);
        }

        RefreshShop();
    }
}