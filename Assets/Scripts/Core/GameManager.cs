using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnLevelUp;
    // UI olmadığı için şimdilik OnStatsChanged olayına ihtiyacımız yok, ama ileride lazım olacak.
    // public static event Action OnStatsChanged;

    [Header("Oyun İlerleme Ayarları")]
    [Tooltip("Tüm oyun seviyeleri buraya sırasıyla atanmalıdır.")]
    [SerializeField]
    private LevelData[] levelProgression;

    // Oyuncu verileri
    public int TotalCoins { get; private set; }
    public int TotalClicks { get; private set; }
    public int CurrentLevelIndex { get; private set; }

    // Mevcut seviyenin LevelData'sına hızlı erişim sağlar.
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
        // TODO: Verileri yükle (SaveManager). Şimdilik sıfırdan başlıyoruz.
        LoadGame();
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        // UI olmadığından, ilerlemeyi konsoldan takip edelim.
        Debug.Log($"Altın eklendi: +{amount}. Toplam Altın: {TotalCoins}");
        CheckForLevelUp();
    }

    public void AddClicks(int amount)
    {
        TotalClicks += amount;
        Debug.Log($"Tıklama eklendi: +{amount}. Toplam Tıklama: {TotalClicks}");
    }

    private void CheckForLevelUp()
    {
        LevelData currentLevel = GetCurrentLevelData();
        // Eğer son seviyede değilsek ve seviye verisi geçerliyse...
        if (currentLevel != null && CurrentLevelIndex < levelProgression.Length - 1)
        {
            if (TotalCoins >= currentLevel.goldToReachNextLevel)
            {
                LevelUp();
            }
        }
    }

    private void LevelUp()
    {
        CurrentLevelIndex++;
        OnLevelUp?.Invoke();
        Debug.LogWarning($"SEVİYE ATLANDI! Yeni Seviye: {CurrentLevelIndex + 1}");
    }

    // Şimdilik basit başlangıç değerleri atayan metodlar.
    private void LoadGame()
    {
        // TODO: PlayerPrefs'ten veri çekilecek.
        TotalCoins = 0;
        TotalClicks = 0;
        CurrentLevelIndex = 0;
    }
}