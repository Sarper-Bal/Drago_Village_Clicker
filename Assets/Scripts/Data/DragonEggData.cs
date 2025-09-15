using UnityEngine;

// Bu satır, Unity editöründe "Create > Game > Dragon Egg Data" menüsü ekler.
[CreateAssetMenu(fileName = "NewDragonEggData", menuName = "Game/Dragon Egg Data")]
public class DragonEggData : ScriptableObject
{
    [Header("Yumurta Bilgileri")]
    [Tooltip("Dükkanda görünecek olan yumurtanın adı.")]
    public string eggName = "Sıradan Yumurta";

    [Tooltip("Dükkan arayüzünde gösterilecek olan yumurta resmi.")]
    public Sprite eggSprite;

    [Tooltip("Yumurtanın altın maliyeti.")]
    public int cost = 500;

    [Header("Gereksinimler")]
    [Tooltip("Bu yumurtanın dükkanda görünmesi için gereken minimum ejderha seviyesi.")]
    public int requiredPlayerLevel = 1;
}