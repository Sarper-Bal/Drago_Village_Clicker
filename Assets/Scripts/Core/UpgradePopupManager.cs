using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePopupManager : Singleton<UpgradePopupManager>
{
    [Header("Popup Arayüz Elemanları")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    private UpgradeableBuilding currentBuilding;

    public override void Awake()
    {
        base.Awake();
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    private void Start()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        closeButton.onClick.AddListener(HideUpgradePopup);
    }

    public void ShowUpgradePopup(UpgradeableBuilding building)
    {
        currentBuilding = building;
        UpgradeData data = building.CurrentUpgradeData;

        upgradeIcon.sprite = data.icon;
        upgradeNameText.text = data.upgradeName;
        descriptionText.text = data.description;
        costText.text = data.cost.ToString("N0");

        upgradeButton.interactable = (GameManager.Instance.TotalCoins >= data.cost);
        popupPanel.SetActive(true);
    }

    private void HideUpgradePopup()
    {
        popupPanel.SetActive(false);
        currentBuilding = null;
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentBuilding == null || currentBuilding.CurrentUpgradeData == null) return;

        UpgradeData upgradeData = currentBuilding.CurrentUpgradeData;

        if (GameManager.Instance.TotalCoins >= upgradeData.cost)
        {
            GameManager.Instance.SpendGold(upgradeData.cost);
            GameManager.Instance.IncrementBuildingLevel(upgradeData.upgradeID);

            if (!string.IsNullOrEmpty(upgradeData.targetBuildingID))
            {
                VillageManager.Instance.PlayAnimationOnBuilding(upgradeData.targetBuildingID);
            }

            currentBuilding.AdvanceToNextUpgrade();
            HideUpgradePopup();
        }
    }
}