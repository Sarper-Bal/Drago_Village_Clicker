using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePopupManager : Singleton<UpgradePopupManager>
{
    public GameObject popupPanel;
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Image iconImage;
    public Button upgradeButton;
    public Button closeButton;

    private UpgradeableBuilding currentBuilding;

    private void Start()
    {
        popupPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowUpgradePopup(UpgradeableBuilding building)
    {
        currentBuilding = building;
        UpgradeData data = building.CurrentUpgradeData;

        if (data == null)
        {
            Debug.LogError("Geliştirme verisi bulunamadı!");
            return;
        }

        upgradeNameText.text = data.upgradeName;
        descriptionText.text = data.description;
        costText.text = data.cost.ToString("N0");
        iconImage.sprite = data.icon;

        // HATA GİDERİLDİ: TotalCoins yerine GetCurrentVillageCoins kullanıldı.
        upgradeButton.interactable = GameManager.Instance.GetCurrentVillageCoins() >= data.cost;
        popupPanel.SetActive(true);
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentBuilding != null)
        {
            UpgradeData data = currentBuilding.CurrentUpgradeData;
            // HATA GİDERİLDİ: TotalCoins yerine GetCurrentVillageCoins kullanıldı.
            if (GameManager.Instance.GetCurrentVillageCoins() >= data.cost)
            {
                GameManager.Instance.SpendGold(data.cost);
                GameManager.Instance.IncrementBuildingLevel(data.upgradeID);

                // Bina animasyonunu tetikle
                if (VillageManager.Instance != null && !string.IsNullOrEmpty(data.targetBuildingID))
                {
                    VillageManager.Instance.PlayAnimationOnBuilding(data.targetBuildingID);
                }

                currentBuilding.AdvanceToNextUpgrade();
            }
        }
        ClosePopup();
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
        currentBuilding = null;
    }
}