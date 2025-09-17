using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Sahne yönetimi için eklendi.

public class PassiveIncomeManager : Singleton<PassiveIncomeManager>
{
    [Header("Veri Referansları")]
    [Tooltip("Oyundaki TÜM geliştirme (UpgradeData) dosyaları buraya atanmalıdır.")]
    [SerializeField] private List<UpgradeData> allUpgrades;

    private int totalGoldPerSecond = 0;
    private float timer = 0f;

    // --- YENİ EKLENEN: Sahnedeki binaların listesini tutacak. ---
    private List<UpgradeableBuilding> activeBuildingsInScene = new List<UpgradeableBuilding>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yeni sahne yüklendiğinde, binaları bul ve geliri yeniden hesapla.
        FindAllUpgradeableBuildings();
        RecalculateTotalIncome();
    }

    void Update()
    {
        if (totalGoldPerSecond == 0) return;
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            GameManager.Instance.AddCoins(totalGoldPerSecond);

            // --- YENİ EKLENEN: Her altın eklendiğinde animasyonları tetikle. ---
            TriggerBuildingAnimations();

            timer -= 1f;
        }
    }

    // --- YENİ METOT: Sahnedeki tüm geliştirilebilir binaları bulur. ---
    private void FindAllUpgradeableBuildings()
    {
        activeBuildingsInScene.Clear();
        activeBuildingsInScene.AddRange(FindObjectsByType<UpgradeableBuilding>(FindObjectsSortMode.None));
    }

    // --- YENİ METOT: Tüm binaların animasyonunu oynatır. ---
    private void TriggerBuildingAnimations()
    {
        foreach (var building in activeBuildingsInScene)
        {
            building.PlayActiveAnimation();
        }
    }

    public void RecalculateTotalIncome()
    {
        totalGoldPerSecond = 0;

        VillageData currentVillage = GameManager.Instance.GetCurrentVillageData();
        if (currentVillage == null) return;

        string currentVillageID = currentVillage.villageID;

        if (GameManager.Instance.BuildingLevels.TryGetValue(currentVillageID, out var currentVillageBuildings))
        {
            foreach (var buildingEntry in currentVillageBuildings)
            {
                string upgradeID = buildingEntry.Key;
                int level = buildingEntry.Value;
                totalGoldPerSecond += GetTotalIncomeForUpgrade(upgradeID, level);
            }
        }
        Debug.Log($"Toplam Saniye Başına Gelir Yeniden Hesaplandı: {totalGoldPerSecond}");
    }

    private int GetTotalIncomeForUpgrade(string upgradeID, int currentLevel)
    {
        int income = 0;
        UpgradeData currentUpgradeData = allUpgrades.Find(u => u.upgradeID == upgradeID);

        for (int i = 0; i < currentLevel; i++)
        {
            if (currentUpgradeData != null)
            {
                income += currentUpgradeData.goldPerSecondBonus;
                currentUpgradeData = currentUpgradeData.nextUpgrade;
            }
        }
        return income;
    }
}