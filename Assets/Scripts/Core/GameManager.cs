using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening; // DOTween'i kullanabilmek için bu kütüphaneyi ekliyoruz.

public class GameManager : Singleton<GameManager>
{
    public static event Action OnGameReady;
    public static event Action OnLevelUp;
    public static event Action OnStatsChanged;
    public static event Action<bool> OnLevelUpReady;

    [Header("Genel Oyun Verileri")]
    [Tooltip("Oyunda var olan tüm köylerin ana veri dosyaları buraya atanmalıdır.")]
    [SerializeField] private List<VillageData> allVillages;

    public int TotalCoins { get; private set; }
    public int TotalClicks { get; private set; }

    private List<string> unlockedVillageIDs = new List<string>();
    private Dictionary<string, int> villageDragonLevels = new Dictionary<string, int>();
    private string currentVillageID;

    private bool isReadyToLevelUp = false;

    void Start()
    {
        LoadGame();
        OnGameReady?.Invoke();
    }

    public VillageData GetCurrentVillageData()
    {
        return allVillages.FirstOrDefault(v => v.villageID == currentVillageID);
    }

    public LevelData GetCurrentDragonLevelData()
    {
        VillageData activeVillage = GetCurrentVillageData();
        if (activeVillage == null) return null;

        int dragonLevel = GetCurrentDragonLevel();
        if (dragonLevel >= 0 && dragonLevel < activeVillage.villageLevelProgression.Count)
        {
            return activeVillage.villageLevelProgression[dragonLevel];
        }
        return null;
    }

    public int GetCurrentDragonLevel()
    {
        villageDragonLevels.TryGetValue(currentVillageID, out int level);
        return level;
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
    }

    public void SpendGold(int amount)
    {
        if (TotalCoins >= amount) TotalCoins -= amount;
        OnStatsChanged?.Invoke();
    }

    private void CheckForLevelUpCondition()
    {
        if (isReadyToLevelUp) return;

        VillageData village = GetCurrentVillageData();
        LevelData currentLevel = GetCurrentDragonLevelData();

        if (village != null && currentLevel != null && GetCurrentDragonLevel() < village.villageLevelProgression.Count - 1)
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
        LevelData currentLevel = GetCurrentDragonLevelData();
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
        villageDragonLevels[currentVillageID]++;
        OnLevelUp?.Invoke();
        OnLevelUpReady?.Invoke(false);
        OnStatsChanged?.Invoke();
    }

    public void UnlockAndSwitchToVillage(VillageData villageToUnlock)
    {
        if (villageToUnlock == null || unlockedVillageIDs.Contains(villageToUnlock.villageID)) return;

        unlockedVillageIDs.Add(villageToUnlock.villageID);
        villageDragonLevels.Add(villageToUnlock.villageID, 0);

        SwitchToVillage(villageToUnlock.villageID);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip köyün sahnesini yükler.
    /// </summary>
    public void SwitchToVillage(string villageID)
    {
        VillageData village = allVillages.FirstOrDefault(v => v.villageID == villageID);
        if (village != null && !string.IsNullOrEmpty(village.sceneToLoad))
        {
            currentVillageID = village.villageID;

            // --- YENİ EKLENEN SATIR ---
            // Yeni sahneyi yüklemeden HEMEN ÖNCE, çalışan tüm DOTween animasyonlarını durdur.
            // Bu, "Target is missing" hatasını tamamen engeller.
            DOTween.KillAll();

            SceneManager.LoadScene(village.sceneToLoad);
        }
    }

    private void LoadGame()
    {
        TotalCoins = 0;
        TotalClicks = 0;

        unlockedVillageIDs.Clear();
        villageDragonLevels.Clear();

        if (allVillages.Count > 0)
        {
            VillageData firstVillage = allVillages[0];
            unlockedVillageIDs.Add(firstVillage.villageID);
            villageDragonLevels.Add(firstVillage.villageID, 0);
            currentVillageID = firstVillage.villageID;
        }
    }
}