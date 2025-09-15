using UnityEngine;

// Bu script, bir ejderha seviyesinin özelliklerini ve o seviyede açılacak binaları tanımlar.
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Seviye Kimliği")]
    public string levelDescription = "Seviye 1";

    [Header("Seviye Atlama Koşulu")]
    public int goldToReachNextLevel = 100;

    [Header("Ejderha Bilgisi")]
    public DragonData dragonDataForThisLevel;

    // --- EKSİK OLAN VE GERİ EKLENEN ALAN ---
    [Header("Bu Seviyede Açılacak Binalar")]
    public BuildingUnlockData[] buildingsToUnlock;
}