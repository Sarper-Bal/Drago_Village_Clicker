using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    // --- YENİ EKLENEN OLAY ---
    public static event Action OnGameReady; // Oyunun tüm sistemlerinin başlaması için sinyal.

    public static event Action OnLevelUp;
    public static event Action OnStatsChanged;
    public static event Action<bool> OnLevelUpReady;

    [Header("Oyun İlerleme Ayarları")]
    [SerializeField] private LevelData[] levelProgression;

    public int TotalCoins { get; private set; }
    public int TotalClicks { get; private set; }
    public int CurrentLevelIndex { get; private set; }

    private bool isReadyToLevelUp = false;

    void Start()
    {
        LoadGame();

        // --- GÜNCELLENMİŞ BAŞLANGIÇ ---
        // Tüm kurulum bittikten sonra, diğer tüm yöneticilere "başlayın" sinyalini yolla.
        OnGameReady?.Invoke();
    }

    public LevelData GetCurrentLevelData()
    {
        if (CurrentLevelIndex >= 0 && CurrentLevelIndex < levelProgression.Length)
        {
            return levelProgression[CurrentLevelIndex];
        }
        return null;
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        OnStatsChanged?.Invoke();
        CheckForLevelUpCondition();
    }

    public void AddClicks(int amount)
    {
        TotalClicks += amount;
        OnStatsChanged?.Invoke();
    }

    public void SpendGold(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
        }
        OnStatsChanged?.Invoke();
    }

    private void CheckForLevelUpCondition()
    {
        if (isReadyToLevelUp) return;
        LevelData currentLevel = GetCurrentLevelData();
        if (currentLevel != null && CurrentLevelIndex < levelProgression.Length - 1)
        {
            if (TotalCoins >= currentLevel.goldToReachNextLevel)
            {
                isReadyToLevelUp = true;
                OnLevelUpReady?.Invoke(true);
            }
        }
    }

    public void PerformLevelUp()
    {
        if (!isReadyToLevelUp) return;
        LevelData currentLevel = GetCurrentLevelData();
        if (currentLevel == null) return;
        int costToLevelUp = currentLevel.goldToReachNextLevel;
        if (TotalCoins >= costToLevelUp)
        {
            SpendGold(costToLevelUp);
        }
        isReadyToLevelUp = false;
        LevelUp();
    }

    private void LevelUp()
    {
        CurrentLevelIndex++;
        OnLevelUp?.Invoke();
        OnLevelUpReady?.Invoke(false);
        OnStatsChanged?.Invoke();
    }

    private void LoadGame()
    {
        TotalCoins = 0;
        TotalClicks = 0;
        CurrentLevelIndex = 0;
    }
}