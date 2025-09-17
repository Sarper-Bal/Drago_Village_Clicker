using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnGameReady;
    public static event Action OnLevelUp;
    public static event Action OnStatsChanged;
    public static event Action<bool> OnVillageCompletionReady;

    [Header("Oyun Veri Listesi")]
    [SerializeField] private List<VillageData> allVillages;

    // --- YENİ EKLENEN/GERİ GETİRİLEN DEĞİŞKEN ---
    public int TotalClicks { get; private set; }

    private Dictionary<string, int> villageCoins = new Dictionary<string, int>();
    private Dictionary<string, int> villageDragonLevels = new Dictionary<string, int>();
    public Dictionary<string, Dictionary<string, int>> BuildingLevels { get; private set; } = new Dictionary<string, Dictionary<string, int>>();
    private string currentVillageID;
    private bool isReadyForNextVillage = false;

    void Start()
    {
        LoadGame();
        OnGameReady?.Invoke();
    }

    public int GetCurrentVillageCoins()
    {
        villageCoins.TryGetValue(currentVillageID, out int coins);
        return coins;
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

    public void IncrementBuildingLevel(string upgradeID)
    {
        if (!BuildingLevels.ContainsKey(currentVillageID))
        {
            BuildingLevels[currentVillageID] = new Dictionary<string, int>();
        }
        if (!BuildingLevels[currentVillageID].ContainsKey(upgradeID))
        {
            BuildingLevels[currentVillageID][upgradeID] = 1;
        }
        else
        {
            BuildingLevels[currentVillageID][upgradeID]++;
        }

        if (PassiveIncomeManager.Instance != null)
        {
            PassiveIncomeManager.Instance.RecalculateTotalIncome();
        }
    }

    // --- YENİ EKLENEN/GERİ GETİRİLEN METOT ---
    public void AddClicks(int amount)
    {
        TotalClicks += amount;
    }

    public void AddCoins(int amount)
    {
        if (villageCoins.ContainsKey(currentVillageID))
        {
            villageCoins[currentVillageID] += amount;
            OnStatsChanged?.Invoke();
            CheckForVillageCompletion();
        }
    }

    public void SpendGold(int amount)
    {
        if (villageCoins.ContainsKey(currentVillageID) && villageCoins[currentVillageID] >= amount)
        {
            villageCoins[currentVillageID] -= amount;
        }
        OnStatsChanged?.Invoke();
    }

    private void CheckForVillageCompletion()
    {
        if (isReadyForNextVillage) return;

        VillageData village = GetCurrentVillageData();
        if (village != null)
        {
            if (GetCurrentVillageCoins() >= village.goldToCompleteVillage)
            {
                isReadyForNextVillage = true;
                OnVillageCompletionReady?.Invoke(true);
            }
        }
    }

    public void CompleteVillageAndAdvance()
    {
        if (!isReadyForNextVillage) return;

        isReadyForNextVillage = false;
        OnVillageCompletionReady?.Invoke(false);
        AdvanceToNextVillage();
    }

    private void AdvanceToNextVillage()
    {
        int currentIndex = allVillages.FindIndex(v => v.villageID == currentVillageID);
        int nextIndex = currentIndex + 1;

        if (nextIndex < allVillages.Count)
        {
            VillageData nextVillage = allVillages[nextIndex];
            currentVillageID = nextVillage.villageID;

            if (!villageCoins.ContainsKey(currentVillageID))
                villageCoins.Add(currentVillageID, 0);
            else
                villageCoins[currentVillageID] = 0;

            if (!villageDragonLevels.ContainsKey(currentVillageID))
                villageDragonLevels.Add(currentVillageID, 0);
            else
                villageDragonLevels[currentVillageID] = 0;

            DOTween.KillAll();
            SceneManager.LoadScene(nextVillage.sceneToLoad);
        }
        else
        {
            Debug.Log("Oyun Bitti! Tüm köyler tamamlandı.");
        }
    }

    private void LevelUp()
    {
        villageDragonLevels[currentVillageID]++;
        OnLevelUp?.Invoke();
        OnStatsChanged?.Invoke();
    }

    public void PerformDragonLevelUp()
    {
        LevelData currentLevelData = GetCurrentDragonLevelData();
        if (currentLevelData == null) return;

        int costToLevelUp = currentLevelData.goldToReachNextLevel;
        if (GetCurrentVillageCoins() >= costToLevelUp)
        {
            SpendGold(costToLevelUp);
            LevelUp();
        }
    }

    private void LoadGame()
    {
        // TotalClicks'i de sıfırla
        TotalClicks = 0;
        villageCoins.Clear();
        villageDragonLevels.Clear();
        BuildingLevels.Clear();

        if (allVillages.Count > 0)
        {
            VillageData firstVillage = allVillages[0];
            currentVillageID = firstVillage.villageID;
            villageCoins.Add(currentVillageID, 0);
            villageDragonLevels.Add(currentVillageID, 0);
        }
    }
}