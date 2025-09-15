using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu kütüphane gerekli.

public class UIManager : Singleton<UIManager>
{
    // Değişkenler artık public değil, çünkü onları kod ile bulacağız.
    private RectTransform levelUpPanel;
    private TextMeshProUGUI goldText;
    private Button levelUpButton;

    private Sequence panelReadyPulseSequence;
    private bool isReadyForLevelUp = false;
    private Vector3 initialPanelScale;

    private void OnEnable()
    {
        GameManager.OnStatsChanged += UpdateGoldUI;
        GameManager.OnLevelUpReady += SetLevelUpReadyState;
        DragonController.OnDragonClicked += PlayClickAnimation;
        // Yeni bir sahne yüklendiğinde OnSceneLoaded metodunu çağır.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameManager.OnStatsChanged -= UpdateGoldUI;
        GameManager.OnLevelUpReady -= SetLevelUpReadyState;
        DragonController.OnDragonClicked -= PlayClickAnimation;
        // Dinlemeyi bırakmayı unutmuyoruz.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Yeni bir sahne yüklendiğinde çalışır ve UI referanslarını yeniden bulur.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sahnedeki Canvas'ı bul.
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            // Canvas'ın altındaki UI elemanlarını isimleriyle bulup ata.
            // Bu isimlerin Hiyerarşi'deki isimlerle eşleştiğinden emin ol!
            levelUpPanel = canvas.transform.Find("LevelUp_Panel")?.GetComponent<RectTransform>();
            if (levelUpPanel != null)
            {
                goldText = levelUpPanel.transform.Find("Progress_Text")?.GetComponent<TextMeshProUGUI>();
                levelUpButton = levelUpPanel.GetComponent<Button>();

                // Butonun olayını yeniden bağla.
                if (levelUpButton != null)
                {
                    levelUpButton.onClick.RemoveAllListeners();
                    levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
                }

                // Başlangıç boyutunu kaydet ve UI'ı ilk değerlerle güncelle.
                initialPanelScale = levelUpPanel.localScale;
                UpdateGoldUI();
                SetLevelUpReadyState(false); // Yeni sahnede butonu sıfırla.
            }
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText == null) return; // Referans henüz bulunmadıysa hata vermesini engelle.
        goldText.text = GameManager.Instance.TotalCoins.ToString("N0");
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

    // Geri kalan tüm metotlar (SetLevelUpReadyState, PlayClickAnimation, StartReadyPulseAnimation, StopReadyPulseAnimation)
    // aynı kalıyor. Onları buraya eklemiyorum ama script'inizde olmalılar.

    private void SetLevelUpReadyState(bool isReady)
    {
        isReadyForLevelUp = isReady;
        if (levelUpButton == null) return;

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
        if (isReadyForLevelUp || levelUpPanel == null) return;
        levelUpPanel.DOKill();
        levelUpPanel.localScale = initialPanelScale;
        levelUpPanel.DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.2f, 1, 0.5f);
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