using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnLevelUp;
    // --- YENİ EKLENEN OLAYLAR ---
    public static event Action OnStatsChanged; // Altın gibi değerler değiştiğinde tetiklenir.
    public static event Action<bool> OnLevelUpReady; // Seviye atlamaya hazır olup olunmadığını bildirir.

    [Header("Oyun İlerleme Ayarları")]
    [Tooltip("Tüm oyun seviyeleri buraya sırasıyla atanmalıdır.")]
    [SerializeField]
    private LevelData[] levelProgression;

    public int TotalCoins { get; private set; }
    public int TotalClicks { get; private set; }
    public int CurrentLevelIndex { get; private set; }

    // --- YENİ EKLENEN DURUM ---
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
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"Altın eklendi: +{amount}. Toplam Altın: {TotalCoins}");

        OnStatsChanged?.Invoke(); // UI'a "değerler değişti, metnini güncelle" diye haber ver.
        CheckForLevelUpCondition();
    }

    public void AddClicks(int amount)
    {
        TotalClicks += amount;
        OnStatsChanged?.Invoke(); // UI'a haber ver.
    }

    // --- GÜNCELLENMİŞ METOT ---
    private void CheckForLevelUpCondition()
    {
        // Eğer zaten atlamaya hazırsa, tekrar kontrol etmeye gerek yok.
        if (isReadyToLevelUp) return;

        LevelData currentLevel = GetCurrentLevelData();
        if (currentLevel != null && CurrentLevelIndex < levelProgression.Length - 1)
        {
            if (TotalCoins >= currentLevel.goldToReachNextLevel)
            {
                // Koşul sağlandı! Otomatik atlamak yerine durumu değiştir ve UI'a haber ver.
                isReadyToLevelUp = true;
                OnLevelUpReady?.Invoke(true); // UI'a "butonu aktifleştir" sinyalini yolla.
            }
        }
    }

    // --- YENİ EKLENEN METOT ---
    /// <summary>
    /// UIManager tarafından çağrılır. Oyuncu butona tıkladığında seviye atlama işlemini gerçekleştirir.
    /// </summary>
    public void PerformLevelUp()
    {
        // Sadece hazır olduğunda seviye atla.
        if (!isReadyToLevelUp) return;

        isReadyToLevelUp = false;
        LevelUp();
    }

    private void LevelUp()
    {
        CurrentLevelIndex++;
        OnLevelUp?.Invoke(); // DragonSpawner ve VillageManager'a haber ver.
        OnLevelUpReady?.Invoke(false); // UI'a "butonu normale döndür" sinyalini yolla.
        OnStatsChanged?.Invoke(); // UI'a yeni seviyenin hedeflerini göstermesi için haber ver.
        Debug.LogWarning($"SEVİYE ATLANDI! Yeni Seviye: {CurrentLevelIndex + 1}");
    }

    private void LoadGame()
    {
        TotalCoins = 0;
        TotalClicks = 0;
        CurrentLevelIndex = 0;
    }
}