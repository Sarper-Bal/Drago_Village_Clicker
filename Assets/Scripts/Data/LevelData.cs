using UnityEngine;

// Bu satır, Unity editöründe "Create > Game > Level Data" menüsü ekler.
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Seviye İlerleme Bilgisi")]
    public string levelDescription = "Seviye 1"; // Editörde kolayca tanımak için.
    public int goldToReachNextLevel = 100; // Bu seviyeyi tamamlayıp bir sonrakine geçmek için gereken altın.

    [Header("Seviye İçeriği")]
    // Bu seviyede hangi ejderhanın ortaya çıkacağını bu veri belirler.
    public DragonData dragonDataForThisLevel;
}