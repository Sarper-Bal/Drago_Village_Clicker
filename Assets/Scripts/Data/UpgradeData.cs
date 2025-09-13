using UnityEngine;

// Bu satır, Unity editöründe "Create > Game > Upgrade Data" menüsü ekler.
[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Kimlik Bilgileri")]
    [Tooltip("Bu geliştirmenin benzersiz kimliği. Kayıt sisteminde kullanılacak.")]
    public string upgradeID;

    [Header("Mağaza Bilgileri")]
    public string upgradeName = "Yeni Geliştirme";
    [TextArea(3, 5)] // Açıklama alanını Inspector'da daha büyük gösterir.
    public string description = "Bu geliştirmenin açıklaması.";
    public Sprite icon;
    public int cost = 100;

    [Header("Gereksinimler")]
    [Tooltip("Bu geliştirmenin mağazada görünmesi için gereken minimum ejderha seviyesi.")]
    public int minPlayerLevel = 1;

    [Header("Kazançlar")]
    [Tooltip("Bu geliştirme satın alındığında saniye başına ne kadar ek altın kazandıracağı.")]
    public int goldPerSecondBonus = 1;

    [Header("Görsel Efektler")]
    [Tooltip("Satın alım sonrası animasyon oynatılacak binanın kimliği (BuildingSpawnPoint ID'si).")]
    public string targetBuildingID;

    [Header("Geliştirme Zinciri")]
    [Tooltip("Bu geliştirme satın alındıktan sonra mağazada görünecek olan bir sonraki seviye geliştirme.")]
    public UpgradeData nextUpgrade; // B seçeneğini (seviyeli geliştirme) sağlayan en önemli kısım!
}