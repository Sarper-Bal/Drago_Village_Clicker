using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePopupManager : Singleton<UpgradePopupManager>
{
    [Header("Popup Arayüz Elemanları")]
    [Tooltip("Geliştirme bilgilerini gösteren ana panel.")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    // O anda hangi binanın geliştirme panelini açtığımızı aklımızda tutmak için.
    private UpgradeableBuilding currentBuilding;

    public override void Awake()
    {
        base.Awake();
        // Oyun başında panelin kapalı olduğundan emin ol.
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // Butonların tıklama olaylarını ilgili fonksiyonlara bağla.
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        closeButton.onClick.AddListener(HideUpgradePopup);
    }

    /// <summary>
    /// Geliştirme panelini gösterir ve içini doldurur.
    /// UpgradeableBuilding tarafından çağrılır.
    /// </summary>
    public void ShowUpgradePopup(UpgradeableBuilding building)
    {
        // Gelen bina bilgisini sakla.
        currentBuilding = building;
        UpgradeData data = building.CurrentUpgradeData;

        // Paneli doldur.
        upgradeIcon.sprite = data.icon;
        upgradeNameText.text = data.upgradeName;
        descriptionText.text = data.description;
        costText.text = data.cost.ToString("N0");

        // Oyuncunun altını yetiyorsa butonu aktif et.
        upgradeButton.interactable = (GameManager.Instance.TotalCoins >= data.cost);

        // Paneli görünür yap.
        popupPanel.SetActive(true);
        // TODO: DOTween ile güzel bir açılma animasyonu eklenebilir.
    }

    /// <summary>
    /// Geliştirme panelini gizler.
    /// </summary>
    private void HideUpgradePopup()
    {
        popupPanel.SetActive(false);
        currentBuilding = null; // Saklanan bina referansını temizle.
        // TODO: DOTween ile güzel bir kapanma animasyonu eklenebilir.
    }

    /// <summary>
    /// "Yükselt" butonuna tıklandığında çalışır.
    /// </summary>
    private void OnUpgradeButtonClicked()
    {
        // Her ihtimale karşı kontroller.
        if (currentBuilding == null || currentBuilding.CurrentUpgradeData == null) return;

        UpgradeData upgradeData = currentBuilding.CurrentUpgradeData;

        // Altın yeterli mi diye son bir kontrol yap.
        if (GameManager.Instance.TotalCoins >= upgradeData.cost)
        {
            // 1. Altını harca.
            GameManager.Instance.SpendGold(upgradeData.cost);

            // 2. Pasif geliri artır.
            PassiveIncomeManager.Instance.AddGoldPerSecond(upgradeData.goldPerSecondBonus);

            // 3. Bina üzerinde animasyon oynat.
            if (!string.IsNullOrEmpty(upgradeData.targetBuildingID))
            {
                // VillageManager henüz oluşturulmadıysa bu satır hata verir.
                // Şimdilik yoruma alabiliriz veya VillageManager'ı ekleyebiliriz.
                // VillageManager.Instance.PlayAnimationOnBuilding(upgradeData.targetBuildingID);
            }

            // 4. Tıklanan binanın seviyesini ilerlet.
            currentBuilding.AdvanceToNextUpgrade();

            // 5. Paneli kapat.
            HideUpgradePopup();
        }
    }
}