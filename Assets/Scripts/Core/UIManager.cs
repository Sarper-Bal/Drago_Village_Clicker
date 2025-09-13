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

    // Mağaza butonuyla ilgili alanlar ve fonksiyonlar kaldırıldı.

    private Sequence panelReadyPulseSequence;
    private bool isReadyForLevelUp = false;
    private Vector3 initialPanelScale;

    public override void Awake()
    {
        base.Awake();
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

    private void PlayClickAnimation()
    {
        if (isReadyForLevelUp) return;
        levelUpPanel.DOKill();
        levelUpPanel.localScale = initialPanelScale;
        levelUpPanel.DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.2f, 1, 0.5f);
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

    // OnShopButtonClicked metodu kaldırıldı.

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
        if (panelReadyPulseSequence != null && panelReadyPulseSequence.IsActive())
        {
            panelReadyPulseSequence.Kill();
        }
        if (levelUpPanel != null)
        {
            levelUpPanel.localScale = initialPanelScale;
        }
    }
}