using UnityEngine;
using System.Collections; // Coroutine için bu kütüphane gerekli.
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class VillageManager : Singleton<VillageManager>
{
    private Dictionary<string, BuildingSpawnPoint> spawnPoints = new Dictionary<string, BuildingSpawnPoint>();
    private Dictionary<string, GameObject> activeBuildings = new Dictionary<string, GameObject>();

    public override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnLevelUp += OnPlayerLevelUp;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnLevelUp -= OnPlayerLevelUp;
    }

    /// <summary>
    /// Yeni bir sahne yüklendiğinde çalışır.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sahnenin tam olarak yüklenmesini ve tüm objelerin hazır olmasını
        // garantilemek için bir frame bekleyen Coroutine'i başlat.
        StartCoroutine(InitializeVillageAfterSceneLoad());
    }

    private IEnumerator InitializeVillageAfterSceneLoad()
    {
        // 1 frame bekleyerek tüm diğer scriptlerin Awake/OnEnable işlemlerini tamamlamasına izin ver.
        yield return null;

        // Yeni sahneye geçildiğinde eski verileri temizle.
        spawnPoints.Clear();
        activeBuildings.Clear();

        // Yeni sahnedeki spawn noktalarını bul ve kaydet.
        FindAndRegisterAllSpawnPoints();
        // Yeni sahnenin başlangıç binalarını inşa et.
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
            }
        }
    }

    private void BuildInitialBuildings()
    {
        LevelData startingLevelData = GameManager.Instance.GetCurrentDragonLevelData();
        if (startingLevelData == null) return;
        // buildingsToUnlock'ın null olup olmadığını kontrol et
        if (startingLevelData.buildingsToUnlock != null && startingLevelData.buildingsToUnlock.Length > 0)
        {
            foreach (var buildingData in startingLevelData.buildingsToUnlock)
            {
                if (spawnPoints.TryGetValue(buildingData.spawnPointID, out BuildingSpawnPoint targetPoint))
                {
                    InstantiateNewBuilding(buildingData, targetPoint);
                }
            }
        }
    }

    private void OnPlayerLevelUp()
    {
        LevelData currentLevelData = GameManager.Instance.GetCurrentDragonLevelData();
        if (currentLevelData == null) return;
        if (currentLevelData.buildingsToUnlock != null && currentLevelData.buildingsToUnlock.Length > 0)
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
                oldBuilding.transform.DOKill();
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

    public void PlayAnimationOnBuilding(string buildingID)
    {
        if (activeBuildings.TryGetValue(buildingID, out GameObject building))
        {
            building.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0.5f);
        }
    }
}