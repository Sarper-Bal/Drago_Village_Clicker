using UnityEngine;

// Bu satır, Unity editöründe "Create > Game > Dragon Data" menüsü ekler.
[CreateAssetMenu(fileName = "NewDragonData", menuName = "Game/Dragon Data")]
public class DragonData : ScriptableObject
{
    [Header("Ejderha Kimliği ve Görünümü")]
    public string dragonName = "Yavru Ejderha"; // Editörde kolayca tanımak için.
    public GameObject dragonPrefab; // Bu ejderhanın görünümünü ve davranışını içeren prefab.

    [Header("Tıklama Kazançları")]
    public int goldPerPress = 1; // Oyuncu bu ejderhaya her tıkladığında kazanacağı altın.
    public int clicksPerPress = 1; // Her tıklamanın toplam tıklama sayacına kaç olarak ekleneceği.
}