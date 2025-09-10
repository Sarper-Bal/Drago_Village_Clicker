using UnityEngine;
using DG.Tweening;

public class DragonSpawner : MonoBehaviour
{
    [Header("Spawner Ayarları")]
    [Tooltip("Ejderhaların yaratılacağı konum.")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentDragonInstance;

    private void OnEnable()
    {
        GameManager.OnLevelUp += SpawnDragonForCurrentLevel;
    }

    private void OnDisable()
    {
        GameManager.OnLevelUp -= SpawnDragonForCurrentLevel;
    }

    void Start()
    {
        SpawnDragonForCurrentLevel();
    }

    private void SpawnDragonForCurrentLevel()
    {
        if (currentDragonInstance != null)
        {
            GameObject oldDragon = currentDragonInstance;
            oldDragon.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(oldDragon));
        }

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

        // --- DEĞİŞİKLİK BURADA BAŞLIYOR ---

        // 1. Prefab'in orijinal scale değerini bir değişkende sakla.
        Vector3 originalScale = dragonData.dragonPrefab.transform.localScale;

        // 2. Yeni ejderhayı yarat.
        if (spawnPoint == null) spawnPoint = this.transform;
        currentDragonInstance = Instantiate(dragonData.dragonPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // 3. Yaratılan ejderhanın Controller'ına kendi verisini ata.
        DragonController controller = currentDragonInstance.GetComponent<DragonController>();
        if (controller != null)
        {
            controller.dragonData = dragonData;
        }

        // 4. Belirme animasyonunu başlatmadan önce boyutunu sıfırla.
        currentDragonInstance.transform.localScale = Vector3.zero;
        // 5. Animasyonu Vector3.one'a değil, sakladığımız originalScale değerine doğru yap.
        currentDragonInstance.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);

        // --- DEĞİŞİKLİK BURADA BİTİYOR ---

        Debug.Log($"'{dragonData.dragonName}' yaratıldı.");
    }
}