using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DragonSpawner : MonoBehaviour
{
    [Header("Spawner Ayarları")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentDragonInstance;

    private void OnEnable()
    {
        // GameManager hazır olduğunda ilk ejderhayı yarat.
        GameManager.OnGameReady += SpawnNewDragon;
        GameManager.OnLevelUp += HandleLevelUpEvent;
    }

    private void OnDisable()
    {
        GameManager.OnGameReady -= SpawnNewDragon;
        GameManager.OnLevelUp -= HandleLevelUpEvent;
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

        yield return null;

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