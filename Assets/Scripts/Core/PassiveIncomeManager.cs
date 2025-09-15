using UnityEngine;
using System.Collections.Generic;

public class PassiveIncomeManager : Singleton<PassiveIncomeManager>
{
    [Header("Veri Referansları")]
    [Tooltip("Oyundaki TÜM geliştirme (UpgradeData) dosyaları buraya atanmalıdır.")]
    [SerializeField] private List<UpgradeData> allUpgrades;

    private int totalGoldPerSecond = 0;
    private float timer = 0f;

    private void OnEnable()
    {
        // Oyun başladığında ilk hesaplamayı yap.
        GameManager.OnGameReady += RecalculateTotalIncome;
    }

    private void OnDisable()
    {
        GameManager.OnGameReady -= RecalculateTotalIncome;
    }

    void Update()
    {
        if (totalGoldPerSecond == 0) return;
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            GameManager.Instance.AddCoins(totalGoldPerSecond);
            timer -= 1f;
        }
    }

    /// <summary>
    /// GameManager'daki kayıtlara bakarak toplam saniye başına altın kazancını yeniden hesaplar.
    /// </summary>
    public void RecalculateTotalIncome()
    {
        totalGoldPerSecond = 0;

        // GameManager'daki tüm bina seviyesi kayıtlarını gez.
        foreach (var villageEntry in GameManager.Instance.BuildingLevels)
        {
            foreach (var buildingEntry in villageEntry.Value)
            {
                string upgradeID = buildingEntry.Key;
                int level = buildingEntry.Value;

                // Bu binanın tüm seviyelerinin gelirlerini topla.
                totalGoldPerSecond += GetTotalIncomeForUpgrade(upgradeID, level);
            }
        }
        Debug.Log($"Toplam Saniye Başına Gelir Yeniden Hesaplandı: {totalGoldPerSecond}");
    }

    /// <summary>
    /// Belirli bir geliştirmenin, belirtilen seviyeye kadar olan TÜM gelirlerini toplar.
    /// </summary>
    private int GetTotalIncomeForUpgrade(string upgradeID, int currentLevel)
    {
        int income = 0;
        // Tüm geliştirme verileri listesinden bu ID'ye ait olan ilk seviyeyi bul.
        UpgradeData currentUpgradeData = allUpgrades.Find(u => u.upgradeID == upgradeID);

        // Bulunan ilk seviyeden başlayarak, mevcut seviyeye kadar olan tüm bonusları topla.
        for (int i = 0; i < currentLevel; i++)
        {
            if (currentUpgradeData != null)
            {
                income += currentUpgradeData.goldPerSecondBonus;
                currentUpgradeData = currentUpgradeData.nextUpgrade; // Bir sonraki seviyeye geç.
            }
        }
        return income;
    }
}