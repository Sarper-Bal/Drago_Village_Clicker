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

    // Animasyon sekansını saklamak için bir değişken. Bu, optimizasyon için önemlidir.
    private Sequence panelClickSequence;

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
        LevelData currentLevel = GameManager.Instance.GetCurrentLevelData();
        if (currentLevel == null) return;

        long currentGold = GameManager.Instance.TotalCoins;
        long targetGold = currentLevel.goldToReachNextLevel;

        progressText.text = $"{currentGold} / {targetGold}";
    }

    private void SetLevelUpReadyState(bool isReady)
    {
        if (isReady)
        {
            levelUpButton.interactable = true;
            progressText.text = "SEVİYE ATLA!";
            // TODO: Butona dikkat çekici bir animasyon ekle (parlama, büyüme vb.)
        }
        else
        {
            levelUpButton.interactable = false;
            UpdateProgressUI();
        }
    }

    /// <summary>
    /// Ejderhaya tıklandığında panelde daha abartılı ve esnek bir animasyon oynatır.
    /// </summary>
    private void PlayClickAnimation()
    {
        // Optimizasyon: Eğer önceki animasyon hala çalışıyorsa, onu anında bitir.
        // true parametresi, animasyonun son haline atlamasını sağlar, bu da çakışmaları önler.
        if (panelClickSequence != null && panelClickSequence.IsActive())
        {
            panelClickSequence.Complete(true);
        }

        // Yeni bir animasyon zinciri (Sequence) oluşturuyoruz.
        panelClickSequence = DOTween.Sequence();

        // Animasyon zincirini oluşturuyoruz: Daha Dramatik Ezil -> Daha Dramatik Uza -> Abartılı Elastik Yerine Otur
        // Değerler daha abartılı bir etki için artırıldı.
        panelClickSequence.Append(levelUpPanel.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.1f)) // 1. Adım: Ezilme (daha belirgin)
            .Append(levelUpPanel.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.1f)) // 2. Adım: Uzanma (daha belirgin)
            .Append(levelUpPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic).SetLoops(1, LoopType.Yoyo)); // 3. Adım: Abartılı Elastik bir şekilde normale dönme. Loops(1, Yoyo) ile hafif bir zıplama ekledik.

        // Animasyonun bu objeyi hedeflemesini sağlayarak güvenlik ve temizlik katıyoruz.
        panelClickSequence.SetTarget(this);
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }
}