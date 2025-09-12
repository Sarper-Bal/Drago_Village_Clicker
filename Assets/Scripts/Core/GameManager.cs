using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnLevelUp;
    public static event Action OnStatsChanged;
    public static event Action<bool> OnLevelUpReady;

    [Header("Oyun İlerleme Ayarları")]
    [Tooltip("Tüm oyun seviyeleri buraya sırasıyla atanmalıdır.")]
    [SerializeField]
    private LevelData[] levelProgression;

    public int TotalCoins { get; private set; }
    public int TotalClicks { get; private set; }
    public int CurrentLevelIndex { get; private set; }

    private bool isReadyToLevelUp = false;

    public LevelData GetCurrentLevelData()
    {
        if (CurrentLevelIndex >= 0 && CurrentLevelIndex < levelProgression.Length)
        {
            return levelProgression[CurrentLevelIndex];
        }
        return null;
    }

    void Start()
    {
        LoadGame();
        // Oyun başladığında UI'ın doğru veriyi göstermesi için ilk güncellemeyi tetikle.
        OnStatsChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"Altın eklendi: +{amount}. Toplam Altın: {TotalCoins}");

        OnStatsChanged?.Invoke();
        CheckForLevelUpCondition();
    }

    public void AddClicks(int amount)
    {
        TotalClicks += amount;
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

    /// <summary>
    /// UIManager tarafından çağrılır. Oyuncu butona tıkladığında seviye atlama işlemini gerçekleştirir.
    /// </summary>
    public void PerformLevelUp()
    {
        if (!isReadyToLevelUp) return;

        LevelData currentLevel = GetCurrentLevelData();
        if (currentLevel == null) return;

        // --- YENİ EKLENEN MANTIK ---
        // Seviye atlamanın bedelini hesapla ve altını harca.
        int costToLevelUp = currentLevel.goldToReachNextLevel;
        if (TotalCoins >= costToLevelUp)
        {
            TotalCoins -= costToLevelUp; // Altını azalt.
        }
        // --- BİTTİ ---

        isReadyToLevelUp = false;
        LevelUp();
    }

    private void LevelUp()
    {
        CurrentLevelIndex++;
        OnLevelUp?.Invoke();
        OnLevelUpReady?.Invoke(false);
        OnStatsChanged?.Invoke(); // UI'ın yeni altın miktarını ve hedefini göstermesi için güncelle.
        Debug.LogWarning($"SEVİYE ATLANDI! Yeni Seviye: {CurrentLevelIndex + 1}");
    }

    private void LoadGame()
    {
        TotalCoins = 0;
        TotalClicks = 0;
        CurrentLevelIndex = 0;
    }
}