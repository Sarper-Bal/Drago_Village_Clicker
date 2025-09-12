using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [Header("Seviye Atlatma Paneli")]
    [SerializeField] private RectTransform levelUpPanel;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button levelUpButton;

    // --- DEĞİŞİKLİK BURADA ---
    [Header("UI Ayarları")]
    [Tooltip("Altın metninin başına eklenecek metin. İkonu kaldırmak için boş bırakın.")]
    // Varsayılan değeri boş bir string "" yaparak ikonu kaldırdık.
    [SerializeField] private string goldPrefix = "";

    private Sequence panelClickSequence;
    private Sequence panelReadyPulseSequence;
    private bool isReadyForLevelUp = false;

    private void OnEnable()
    {
        GameManager.OnStatsChanged += UpdateProgressUI;
        GameManager.OnLevelUpReady += SetLevelUpReadyState;
        DragonController.OnDragonClicked += PlayClickAnimation;
    }

    private void OnDisable()
    {
        GameManager.OnStatsChanged -= UpdateProgressUI;
        GameManager.OnLevelUpReady -= SetLevelUpReadyState;
        DragonController.OnDragonClicked -= PlayClickAnimation;
    }

    private void Start()
    {
        levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
        SetLevelUpReadyState(false);
        UpdateProgressUI();
    }

    /// <summary>
    /// Paneldeki metni, HER ZAMAN oyuncunun sahip olduğu toplam altın miktarını gösterecek şekilde günceller.
    /// </summary>
    private void UpdateProgressUI()
    {
        long currentGold = GameManager.Instance.TotalCoins;

        // Metni, başında prefix olmadan sadece sayıyı gösterecek şekilde formatla.
        progressText.text = $"{goldPrefix}{currentGold:N0}";
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
            UpdateProgressUI();
        }
    }

    private void PlayClickAnimation()
    {
        if (isReadyForLevelUp) return;

        if (panelClickSequence != null && panelClickSequence.IsActive())
        {
            panelClickSequence.Complete(true);
        }

        panelClickSequence = DOTween.Sequence();
        panelClickSequence.Append(levelUpPanel.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.08f))
            .Append(levelUpPanel.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.08f))
            .Append(levelUpPanel.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutElastic));
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

    private void StartReadyPulseAnimation()
    {
        StopReadyPulseAnimation();
        panelReadyPulseSequence = DOTween.Sequence();
        panelReadyPulseSequence.Append(levelUpPanel.DOScale(1.05f, 0.5f).SetEase(Ease.InOutSine))
            .Append(levelUpPanel.DOScale(1f, 0.5f).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

    private void StopReadyPulseAnimation()
    {
        if (panelReadyPulseSequence != null && panelReadyPulseSequence.IsActive())
        {
            panelReadyPulseSequence.Kill();
        }
        levelUpPanel.localScale = Vector3.one;
    }
}