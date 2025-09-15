using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class DragonSpawner : MonoBehaviour
{
    [Header("Spawner Ayarları")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentDragonInstance;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnLevelUp += HandleLevelUpEvent;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnLevelUp -= HandleLevelUpEvent;
    }

    /// <summary>
    /// Yeni bir sahne yüklendiğinde çalışır.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sahnenin tam olarak yüklenmesini ve tüm objelerin hazır olmasını
        // garantilemek için bir frame bekleyen Coroutine'i başlat.
        StartCoroutine(InitializeDragonAfterSceneLoad());
    }

    private IEnumerator InitializeDragonAfterSceneLoad()
    {
        // 1 frame bekleyerek tüm diğer scriptlerin Awake/OnEnable işlemlerini tamamlamasına izin ver.
        yield return null;

        // Başka bir sahneden geliniyorsa, mevcut ejderhayı yok et.
        if (currentDragonInstance != null)
        {
            Destroy(currentDragonInstance);
        }
        // Yeni sahnenin ejderhasını oluştur.
        SpawnNewDragon();
    }

    private void HandleLevelUpEvent()
    {
        StartCoroutine(LevelUpTransitionRoutine());
    }

    private IEnumerator LevelUpTransitionRoutine()
    {
        if (currentDragonInstance != null)
        {
            DragonController oldDragonController = currentDragonInstance.GetComponent<DragonController>();
            if (oldDragonController != null)
            {
                oldDragonController.DestroyDragon();
            }
            else
            {
                Destroy(currentDragonInstance);
            }
        }

        yield return new WaitForSeconds(0.3f); // Ejderhanın yok olma animasyonuna zaman tanı.

        SpawnNewDragon();
    }

    private void SpawnNewDragon()
    {
        LevelData levelData = GameManager.Instance.GetCurrentDragonLevelData();
        if (levelData == null)
        {
            Debug.LogError("Mevcut ejderha seviyesi için LevelData bulunamadı! GameManager'daki VillageData'nın doğru atandığından emin olun.");
            return;
        }

        DragonData dragonData = levelData.dragonDataForThisLevel;
        if (dragonData == null || dragonData.dragonPrefab == null)
        {
            Debug.LogError($"'{levelData.name}' içindeki DragonData veya ejderha prefab'ı eksik!");
            return;
        }

        Vector3 originalScale = dragonData.dragonPrefab.transform.localScale;
        currentDragonInstance = Instantiate(dragonData.dragonPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        DragonController controller = currentDragonInstance.GetComponent<DragonController>();
        if (controller != null)
        {
            controller.dragonData = dragonData;
        }

        currentDragonInstance.transform.localScale = Vector3.zero;
        currentDragonInstance.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
    }
}