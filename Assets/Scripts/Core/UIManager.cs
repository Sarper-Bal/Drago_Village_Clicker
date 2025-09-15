using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : Singleton<UIManager>
{
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameManager.OnStatsChanged -= UpdateGoldUI;
        GameManager.OnLevelUpReady -= SetLevelUpReadyState;
        DragonController.OnDragonClicked -= PlayClickAnimation;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeUIAfterSceneLoad());
    }

    private IEnumerator InitializeUIAfterSceneLoad()
    {
        yield return null;

        // Sahnedeki Canvas'ı bulmak için yeni ve önerilen metodu kullan.
        Canvas canvas = FindFirstObjectByType<Canvas>(); // <-- DEĞİŞİKLİK BURADA
        if (canvas != null)
        {
            levelUpPanel = canvas.transform.Find("LevelUp_Panel")?.GetComponent<RectTransform>();
            if (levelUpPanel != null)
            {
                goldText = levelUpPanel.transform.Find("Progress_Text")?.GetComponent<TextMeshProUGUI>();
                levelUpButton = levelUpPanel.GetComponent<Button>();

                if (levelUpButton != null)
                {
                    levelUpButton.onClick.RemoveAllListeners();
                    levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
                }

                initialPanelScale = levelUpPanel.localScale;
                UpdateGoldUI();
                SetLevelUpReadyState(false);
            }
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText == null) return;
        goldText.text = GameManager.Instance.TotalCoins.ToString("N0");
    }

    private void OnLevelUpButtonClicked()
    {
        GameManager.Instance.PerformLevelUp();
    }

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