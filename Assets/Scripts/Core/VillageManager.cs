using UnityEngine;
using System.Collections.Generic; // Dictionary kullanmak için bu kütüphane gerekli.
using DG.Tweening; // Animasyonlar için DOTween kütüphanesi.

public class VillageManager : Singleton<VillageManager>
{
    // Bu 'Dictionary', sahnedeki tüm spawn noktalarını ID'leri ile eşleştirerek saklar.
    // Bu sayede "House_Spot_1" ID'sine sahip noktayı anında bulabiliriz. Bu çok hızlı ve optimize bir yöntemdir.
    private Dictionary<string, BuildingSpawnPoint> spawnPoints = new Dictionary<string, BuildingSpawnPoint>();

    // Awake metodu, Start'tan önce çalışır. Singleton'dan geldiği için 'override' anahtar kelimesini kullanıyoruz.
    public override void Awake()
    {
        base.Awake(); // Ana Singleton sınıfının Awake metodunu çalıştırarak kurulumu tamamla.
        FindAndRegisterAllSpawnPoints();
    }

    private void OnEnable()
    {
        // GameManager'daki seviye atlama olayını dinlemeye başla.
        // Seviye atlandığında 'OnPlayerLevelUp' metodu otomatik olarak çalışacak.
        GameManager.OnLevelUp += OnPlayerLevelUp;
    }

    private void OnDisable()
    {
        // Bu script yok edildiğinde veya pasif hale geldiğinde dinlemeyi bırakmayı unutmuyoruz.
        // Bu, hafıza sızıntılarını ve hataları önler.
        GameManager.OnLevelUp -= OnPlayerLevelUp;
    }

    /// <summary>
    /// Sahnedeki 'BuildingSpawnPoint' script'ine sahip tüm objeleri bulur ve Dictionary'e kaydeder.
    /// </summary>
    private void FindAndRegisterAllSpawnPoints()
    {
        // Sahnedeki tüm BuildingSpawnPoint bileşenlerini bul.
        BuildingSpawnPoint[] allPoints = FindObjectsByType<BuildingSpawnPoint>(FindObjectsSortMode.None);

        foreach (var point in allPoints)
        {
            // Eğer spawn point'in ID'si boş değilse ve daha önce eklenmemişse...
            if (!string.IsNullOrEmpty(point.spawnPointID) && !spawnPoints.ContainsKey(point.spawnPointID))
            {
                // ...onu ID'si ile birlikte Dictionary'e ekle.
                spawnPoints.Add(point.spawnPointID, point);
                Debug.Log($"Spawn noktası kaydedildi: {point.spawnPointID}");
            }
            else
            {
                // Hatalı veya çift ID'leri tespit etmek için uyarı ver.
                Debug.LogWarning($"Spawn noktası ID'si boş veya zaten mevcut: {point.name}", point.gameObject);
            }
        }
    }

    /// <summary>
    /// GameManager'dan seviye atlama sinyali geldiğinde bu metot çalışır.
    /// </summary>
    private void OnPlayerLevelUp()
    {
        // GameManager'dan mevcut (yeni ulaşılan) seviyenin verilerini al.
        LevelData currentLevelData = GameManager.Instance.GetCurrentLevelData();
        if (currentLevelData == null) return;

        // Bu seviyede açılacak binalar listesini kontrol et.
        if (currentLevelData.buildingsToUnlock.Length > 0)
        {
            Debug.Log($"{currentLevelData.levelDescription} seviyesine ulaşıldı. Binalar inşa ediliyor...");
            // Listede ne kadar bina varsa, hepsi için inşa sürecini başlat.
            foreach (var buildingData in currentLevelData.buildingsToUnlock)
            {
                BuildBuilding(buildingData);
            }
        }
    }

    /// <summary>
    /// Verilen bina verisine göre binayı ilgili spawn noktasında inşa eder.
    /// </summary>
    /// <param name="dataToBuild">İnşa edilecek binanın prefab'ını ve spawn ID'sini içeren veri.</param>
    private void BuildBuilding(BuildingUnlockData dataToBuild)
    {
        // Veri içindeki ID'ye sahip bir spawn noktası Dictionary'de kayıtlı mı diye kontrol et.
        if (spawnPoints.TryGetValue(dataToBuild.spawnPointID, out BuildingSpawnPoint targetPoint))
        {
            // Eğer spawn noktası bulunduysa, binayı o noktanın pozisyonunda yarat (Instantiate).
            // Binayı, düzenli olması için spawn noktasının 'child' objesi yapıyoruz.
            GameObject newBuilding = Instantiate(dataToBuild.buildingPrefab, targetPoint.transform.position, Quaternion.identity, targetPoint.transform);

            // İnşa animasyonunu başlat.
            PlayBuildAnimation(newBuilding);
        }
        else
        {
            // LevelData'da belirtilen ID'ye sahip bir spawn noktası sahnede bulunamazsa hata ver.
            // Bu, olası hataları anında fark etmemizi sağlar.
            Debug.LogError($"Bina inşa edilemedi! Sahne_de '{dataToBuild.spawnPointID}' ID'sine sahip bir spawn noktası bulunamadı.");
        }
    }

    /// <summary>
    /// Binanın "inşa ediliyor" gibi görünmesini sağlayan DOTween animasyonunu oynatır.
    /// </summary>
    /// <param name="buildingObject">Animasyonun uygulanacağı bina objesi.</param>
    private void PlayBuildAnimation(GameObject buildingObject)
    {
        // Binanın orijinal boyutunu sakla.
        Vector3 originalScale = buildingObject.transform.localScale;
        // Animasyon başlamadan önce boyutunu sıfır yap.
        buildingObject.transform.localScale = Vector3.zero;

        // Binanın boyutunu sıfırdan orijinal boyutuna 0.5 saniyede, güzel bir efektle getir.
        buildingObject.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
    }
}