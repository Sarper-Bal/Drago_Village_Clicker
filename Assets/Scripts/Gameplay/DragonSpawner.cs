using UnityEngine;
using DG.Tweening; // EKLENDİ: DOTween kütüphanesini bu scripte dahil ediyoruz.

public class DragonSpawner : MonoBehaviour
{
    [Header("Spawner Ayarları")]
    [Tooltip("Ejderhaların yaratılacağı konum.")]
    [SerializeField] private Transform spawnPoint;

    // Sahnede o an aktif olan ejderha objesini aklımızda tutmak için.
    private GameObject currentDragonInstance;

    private void OnEnable()
    {
        // GameManager'dan seviye atlama olayını dinlemeye başla.
        GameManager.OnLevelUp += SpawnDragonForCurrentLevel;
    }

    private void OnDisable()
    {
        // Dinlemeyi bırakmayı unutma!
        GameManager.OnLevelUp -= SpawnDragonForCurrentLevel;
    }

    void Start()
    {
        // Oyun başladığında ilk ejderhayı yarat.
        SpawnDragonForCurrentLevel();
    }

    /// <summary>
    /// Mevcut seviyeye uygun ejderhayı yaratır.
    /// </summary>
    private void SpawnDragonForCurrentLevel()
    {
        // 1. Önceki ejderhayı yok et (eğer varsa).
        if (currentDragonInstance != null)
        {
            // DOTween ile küçük bir yok olma efekti ekleyebiliriz.
            currentDragonInstance.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack) // Artık 'Ease' tanınacak.
                .OnComplete(() => Destroy(currentDragonInstance));
        }

        // 2. GameManager'dan mevcut seviye verisini al.
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

        // 3. Yeni ejderhayı spawn point'te yarat (Instantiate).
        if (spawnPoint == null) spawnPoint = this.transform; // Spawn point atanmamışsa, spawner'ın kendi konumunu kullan.

        currentDragonInstance = Instantiate(dragonData.dragonPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // 4. Yaratılan ejderhanın Controller'ına kendi verisini ata.
        DragonController controller = currentDragonInstance.GetComponent<DragonController>();
        if (controller != null)
        {
            controller.dragonData = dragonData;
        }

        // DOTween ile küçük bir belirme efekti.
        currentDragonInstance.transform.localScale = Vector3.zero;
        currentDragonInstance.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Artık 'Ease' tanınacak.

        Debug.Log($"'{dragonData.dragonName}' yaratıldı.");
    }
}