using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Seviye İlerleme Bilgisi")]
    public string levelDescription = "Seviye 1";
    public int goldToReachNextLevel = 100;

    [Header("Seviye İçeriği")]
    public DragonData dragonDataForThisLevel;

    // --- YENİ SATIR AŞAĞIDA ---
    [Header("Bu Seviyede Açılacak Binalar")]
    public BuildingUnlockData[] buildingsToUnlock; // O seviyeye ulaşıldığında inşa edilecek binaların listesi.
}