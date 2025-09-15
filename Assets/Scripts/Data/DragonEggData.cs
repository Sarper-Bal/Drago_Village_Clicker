using UnityEngine;

[CreateAssetMenu(fileName = "NewDragonEggData", menuName = "Game/Dragon Egg Data")]
public class DragonEggData : ScriptableObject
{
    [Header("Yumurta Bilgileri")]
    public string eggName = "Sıradan Yumurta";
    public Sprite eggSprite;
    public int cost = 500;

    [Header("Gereksinimler")]
    public int requiredPlayerLevel = 1;

    // --- YENİ EKLENEN ALAN ---
    [Header("Kilit Açma")]
    [Tooltip("Bu yumurta satın alındığında kilidini açacağı köy verisi. Eğer bir köy açmıyorsa boş bırak.")]
    public VillageData villageToUnlock;
}