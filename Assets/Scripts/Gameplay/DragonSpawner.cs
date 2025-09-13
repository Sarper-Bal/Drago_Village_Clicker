using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DragonSpawner : MonoBehaviour
{
    [Header("Spawner Ayarları")]
    [Tooltip("Ejderhaların yaratılacağı konum.")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentDragonInstance;

    private void OnEnable()
    {
        // --- GÜNCELLENMİŞ OLAY DİNLEME ---
        GameManager.OnGameReady += SpawnNewDragon; // Oyun başladığında ilk ejderhayı yarat.
        GameManager.OnLevelUp += HandleLevelUpEvent;
    }

    private void OnDisable()
    {
        // --- GÜNCELLENMİŞ OLAY DİNLEME ---
        GameManager.OnGameReady -= SpawnNewDragon;
        GameManager.OnLevelUp -= HandleLevelUpEvent;
    }

    // --- Start() METODU SİLİNDİ ---

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
        LevelData levelData = GameManager.Instance.GetCurrentLevelData();
        if (levelData == null)
        {
            Debug.LogError("Mevcut seviye için LevelData bulunamadı!");
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
        Debug.Log($"'{dragonData.dragonName}' yaratıldı.");
    }
}