using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [Header("Seviye Atlatma Paneli")]
    [SerializeField] private RectTransform levelUpPanel;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button levelUpButton;

    [Header("Alt Bar Butonları")]
    [SerializeField] private Button shopButton;

    private Sequence panelReadyPulseSequence;
    private bool isReadyForLevelUp = false;
    private Vector3 initialPanelScale;

    public override void Awake()
    {
        base.Awake();
        // Panelin başlangıç boyutunu, animasyonlarda referans olarak kullanmak üzere saklıyoruz.
        if (levelUpPanel != null)
        {
            initialPanelScale = levelUpPanel.localScale;
        }
    }

    private void OnEnable()
    {
        GameManager.OnStatsChanged += UpdateGoldUI;
        GameManager.OnLevelUpReady += SetLevelUpReadyState;
        DragonController.OnDragonClicked += PlayClickAnimation;
    }

    private void OnDisable()
    {
        GameManager.OnStatsChanged -= UpdateGoldUI;
        GameManager.OnLevelUpReady -= SetLevelUpReadyState;
        DragonController.OnDragonClicked -= PlayClickAnimation;
    }

    private void Start()
    {
        levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);

        SetLevelUpReadyState(false);
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        goldText.text = GameManager.Instance.TotalCoins.ToString("N0");
    }

    private void SetLevelUpReadyState(bool isReady)
    {
        isReadyForLevelUp = isReady;

        if (isReady)
        {
            levelUpButton.interactable = true;
            StartReadyPulseAnimation();
        }
        else
        {
            levelUpButton.interactable = false;
            StopReadyPulseAnimation();
            UpdateGoldUI();
        }
    }

    /// <summary>
    /// Ejderhaya tıklandığında panelde animasyon oynatır. Hatalı ChangeStartValue çağrısı kaldırıldı.
    /// </summary>
    private void PlayClickAnimation()
    {
        if (isReadyForLevelUp) return;

        // --- DÜZELTİLMİŞ BLOK ---
        // 1. Önce, panel üzerinde çalışan tüm animasyonları anında durdur.
        levelUpPanel.DOKill();
        // 2. Panelin boyutunu, animasyonun doğru başlaması için orijinal boyutuna sıfırla.
        levelUpPanel.localScale = initialPanelScale;

        // 3. Şimdi "punch" animasyonunu güvenle başlat. Hatalı .ChangeStartValue() çağrısı kaldırıldı.
        levelUpPanel.DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.2f, 1, 0.5f);
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

    private void OnShopButtonClicked()
    {
        ShopManager.Instance.OpenShopPanel();
    }

    private void StartReadyPulseAnimation()
    {
        StopReadyPulseAnimation();
        panelReadyPulseSequence = DOTween.Sequence();
        panelReadyPulseSequence.Append(levelUpPanel.DOScale(initialPanelScale * 1.1f, 0.7f).SetEase(Ease.InOutSine))
            .Append(levelUpPanel.DOScale(initialPanelScale, 0.7f).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

    private void StopReadyPulseAnimation()
    {
        // Null referans hatası almamak için Sequence'in var olup olmadığını kontrol et.
        if (panelReadyPulseSequence != null && panelReadyPulseSequence.IsActive())
        {
            panelReadyPulseSequence.Kill();
        }
        levelUpPanel.localScale = initialPanelScale;
    }
}