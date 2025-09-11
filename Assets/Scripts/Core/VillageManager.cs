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

    private void FindAndRegisterAllSpawnPoints()
    {
        BuildingSpawnPoint[] allPoints = FindObjectsByType<BuildingSpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in allPoints)
        {
            if (!string.IsNullOrEmpty(point.spawnPointID) && !spawnPoints.ContainsKey(point.spawnPointID))
            {
                spawnPoints.Add(point.spawnPointID, point);
            }
            else
            {
                Debug.LogWarning($"Spawn noktası ID'si boş veya zaten mevcut: {point.name}", point.gameObject);
            }
        }
    }

    private void OnPlayerLevelUp()
    {
        LevelData currentLevelData = GameManager.Instance.GetCurrentLevelData();
        if (currentLevelData == null) return;

        if (currentLevelData.buildingsToUnlock.Length > 0)
        {
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
                // --- OPTİMİZASYON VE DÜZELTME ---
                // Burada da aynı mantığı kullanıyoruz. 'oldBuilding' referansı zaten doğru objeyi tutuyor.
                // Animasyon bittikten sonra yenisini inşa etmesi için bir "callback" kullanıyoruz.
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