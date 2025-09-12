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

    [Header("UI Ayarları")]
    [Tooltip("Altın metninin başına eklenecek metin. İkonu kaldırmak için boş bırakın.")]
    [SerializeField] private string goldPrefix = "";

    private Sequence panelClickSequence;

    // --- YENİ EKLENEN ANİMASYON KONTROLLERİ ---
    private Sequence panelReadyPulseSequence; // Hazır animasyonunu saklamak için.
    private bool isReadyForLevelUp = false; // Panelin mevcut durumunu tutar.
    private Vector3 initialPanelScale; // Panelin orijinal boyutunu saklamak için.

    public override void Awake()
    {
        base.Awake();
        // Panelin başlangıç boyutunu sakla.
        initialPanelScale = levelUpPanel.localScale;
    }

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

    private void UpdateProgressUI()
    {
        if (isReadyForLevelUp) return;
        long currentGold = GameManager.Instance.TotalCoins;
        progressText.text = $"{goldPrefix}{currentGold:N0}";
    }

    /// <summary>
    /// Seviye atlamaya hazır olunduğunda butonun durumunu ve animasyonunu değiştirir.
    /// </summary>
    private void SetLevelUpReadyState(bool isReady)
    {
        isReadyForLevelUp = isReady;

        if (isReady)
        {
            levelUpButton.interactable = true;
            StartReadyPulseAnimation(); // Hazır animasyonunu başlat.
        }
        else
        {
            levelUpButton.interactable = false;
            StopReadyPulseAnimation(); // Hazır animasyonunu durdur.
            UpdateProgressUI();
        }
    }

    private void PlayClickAnimation()
    {
        // Eğer seviye atlamaya hazırsa, jöle animasyonunu OYNATMA.
        if (isReadyForLevelUp) return;

        if (panelClickSequence != null && panelClickSequence.IsActive())
        {
            panelClickSequence.Complete(true);
        }

        panelClickSequence = DOTween.Sequence();
        panelClickSequence.Append(levelUpPanel.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.08f))
            .Append(levelUpPanel.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.08f))
            .Append(levelUpPanel.DOScale(initialPanelScale, 0.4f).SetEase(Ease.OutElastic));
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

    // --- YENİ VE GÜNCELLENMİŞ METOTLAR ---
    /// <summary>
    /// Panelin sürekli ve büyük bir şekilde büyüyüp küçüldüğü "hazır" animasyonunu başlatır.
    /// </summary>
    private void StartReadyPulseAnimation()
    {
        StopReadyPulseAnimation(); // Önce varsa eskiyi durdur.

        panelReadyPulseSequence = DOTween.Sequence();
        // Paneli 2 katı büyüklüğe çıkarıp sonra orijinal boyutuna döndürür ve bunu sonsuz döngüde yapar.
        panelReadyPulseSequence.Append(levelUpPanel.DOScale(initialPanelScale * 2f, 0.7f).SetEase(Ease.InOutSine))
            .Append(levelUpPanel.DOScale(initialPanelScale, 0.7f).SetEase(Ease.InOutSine))
            .SetLoops(-1); // Sonsuz döngü.
    }

    /// <summary>
    /// "Hazır" animasyonunu durdurur ve paneli orijinal boyutuna döndürür.
    /// </summary>
    private void StopReadyPulseAnimation()
    {
        if (panelReadyPulseSequence != null && panelReadyPulseSequence.IsActive())
        {
            panelReadyPulseSequence.Kill();
        }
        // Panelin boyutunu, oyunun başında kaydettiğimiz orijinal boyutuna anında sıfırla.
        levelUpPanel.localScale = initialPanelScale;
    }
}