using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class VillageManager : Singleton<VillageManager>
{
    private Dictionary<string, BuildingSpawnPoint> spawnPoints = new Dictionary<string, BuildingSpawnPoint>();
    private Dictionary<string, GameObject> activeBuildings = new Dictionary<string, GameObject>();

    public override void Awake()
    {
        base.Awake();
        FindAndRegisterAllSpawnPoints();
    }

    private void OnEnable()
    {
        GameManager.OnLevelUp += OnPlayerLevelUp;
    }

    private void OnDisable()
    {
        GameManager.OnLevelUp -= OnPlayerLevelUp;
    }

    // --- YENİ EKLENEN METOT ---
    // Start metodu, sahnedeki tüm Awake'ler bittikten sonra, ilk frame güncellenmeden önce bir kez çalışır.
    // Bu, oyunun başlangıç durumunu kurmak için mükemmel bir yerdir.
    private void Start()
    {
        // Oyun başladığında, 1. seviyeye ait binaları inşa etmek için bu fonksiyonu çağırıyoruz.
        BuildInitialBuildings();
    }

    private void FindAndRegisterAllSpawnPoints()
    {
        BuildingSpawnPoint[] allPoints = FindObjectsByType<BuildingSpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in allPoints)
        {
            if (!string.IsNullOrEmpty(point.spawnPointID) && !spawnPoints.ContainsKey(point.spawnPointID))
            {
                spawnPoints.Add(point.spawnPointID, point);
                Debug.Log($"Spawn noktası kaydedildi: {point.spawnPointID}");
            }
            else
            {
                Debug.LogWarning($"Spawn noktası ID'si boş veya zaten mevcut: {point.name}", point.gameObject);
            }
        }
    }

    // --- YENİ EKLENEN METOT ---
    /// <summary>
    /// Oyunun başlangıcında, mevcut seviyeye ait binaları inşa eder.
    /// </summary>
    private void BuildInitialBuildings()
    {
        // GameManager'dan başlangıç seviyesinin verilerini alıyoruz (Level 1).
        LevelData startingLevelData = GameManager.Instance.GetCurrentLevelData();
        if (startingLevelData == null) return;

        // Eğer başlangıç seviyesinde inşa edilecek binalar varsa...
        if (startingLevelData.buildingsToUnlock.Length > 0)
        {
            Debug.Log("Başlangıç binaları inşa ediliyor...");
            // ...her birini sırayla inşa et.
            foreach (var buildingData in startingLevelData.buildingsToUnlock)
            {
                // Zaten var olan BuildBuilding metodunu kullanıyoruz, ama bu sefer yükseltme kontrolü olmadan
                // direkt inşa etmesi için InstantiateNewBuilding'i çağırıyoruz.
                // Bu, oyun her başladığında temiz bir kurulum sağlar.
                if (spawnPoints.TryGetValue(buildingData.spawnPointID, out BuildingSpawnPoint targetPoint))
                {
                    InstantiateNewBuilding(buildingData, targetPoint);
                }
                else
                {
                    Debug.LogError($"Bina inşa edilemedi! Sahnede '{buildingData.spawnPointID}' ID'sine sahip bir spawn noktası bulunamadı.");
                }
            }
        }
    }

    private void OnPlayerLevelUp()
    {
        LevelData currentLevelData = GameManager.Instance.GetCurrentLevelData();
        if (currentLevelData == null) return;

        if (currentLevelData.buildingsToUnlock.Length > 0)
        {
            Debug.Log($"{currentLevelData.levelDescription} seviyesine ulaşıldı. Binalar inşa ediliyor...");
            foreach (var buildingData in currentLevelData.buildingsToUnlock)
            {
                BuildBuilding(buildingData);
            }
        }
    }

    private void BuildBuilding(BuildingUnlockData dataToBuild)
    {
        if (spawnPoints.TryGetValue(dataToBuild.spawnPointID, out BuildingSpawnPoint targetPoint))
        {
            if (activeBuildings.TryGetValue(dataToBuild.spawnPointID, out GameObject oldBuilding))
            {
                oldBuilding.transform.DOKill(); // Animasyon çakışmasını önlemek için eski binanın animasyonlarını durdur.
                oldBuilding.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(oldBuilding);
                    InstantiateNewBuilding(dataToBuild, targetPoint);
                });
            }
            else
            {
                InstantiateNewBuilding(dataToBuild, targetPoint);
            }
        }
        else
        {
            Debug.LogError($"Bina inşa edilemedi! Sahnede '{dataToBuild.spawnPointID}' ID'sine sahip bir spawn noktası bulunamadı.");
        }
    }

    private void InstantiateNewBuilding(BuildingUnlockData dataToBuild, BuildingSpawnPoint targetPoint)
    {
        GameObject newBuilding = Instantiate(dataToBuild.buildingPrefab, targetPoint.transform.position, Quaternion.identity, targetPoint.transform);
        activeBuildings[dataToBuild.spawnPointID] = newBuilding;
        PlayBuildAnimation(newBuilding);
    }

    private void PlayBuildAnimation(GameObject buildingObject)
    {
        Vector3 originalScale = buildingObject.transform.localScale;
        buildingObject.transform.localScale = Vector3.zero;
        buildingObject.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
    }
}