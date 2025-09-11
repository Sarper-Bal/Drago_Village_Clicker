using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class DragonController : MonoBehaviour, IPointerClickHandler
{
    public DragonData dragonData;
    private Vector3 initialScale;

    [Header("Tıklama Animasyon Ayarları")]
    [SerializeField] private float clickAnimationDuration = 0.2f;
    [SerializeField] private float scaleMultiplier = 1.15f;
    [SerializeField] private float shakeStrength = 0.1f;

    // --- YENİ EKLENEN ALANLAR ---
    [Header("Coin Fırlatma Efekti")]
    [Tooltip("Fırlatılacak olan görsel coin prefab'ı.")]
    [SerializeField] private GameObject coinFxPrefab;
    [Tooltip("Coin'in fırlatılacağı alanın yarıçapı.")]
    [SerializeField] private float coinSpawnRadius = 2f;
    // --- BİTTİ ---

    private Sequence clickSequence;
    private bool isDying = false;

    void Start()
    {
        initialScale = transform.localScale;
        if (dragonData == null)
        {
            Debug.LogError("DragonController'a DragonData atanmamış! Lütfen Spawner'ı kontrol edin.", this.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDying || dragonData == null) return;

        PlayClickFeedbackAnimation();
        ProcessGameLogic();
    }

    private void PlayClickFeedbackAnimation()
    {
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
        }

        clickSequence = DOTween.Sequence();

        clickSequence.Append(transform.DOPunchScale(initialScale * (scaleMultiplier - 1), clickAnimationDuration, 1, 0.5f))
            .Join(transform.DOShakePosition(clickAnimationDuration, new Vector3(shakeStrength, shakeStrength, 0), 10, 90, false, true))
            .SetTarget(this);
    }

    private void ProcessGameLogic()
    {
        GameManager.Instance.AddCoins(dragonData.goldPerPress);
        GameManager.Instance.AddClicks(dragonData.clicksPerPress);

        // --- YENİ EKLENEN MANTIK ---
        SpawnCoinEffect();
        // --- BİTTİ ---

        Debug.Log(gameObject.name + " tıklandı!");
    }

    // --- YENİ EKLENEN METOT ---
    /// <summary>
    /// Ejderhanın etrafında rastgele bir konuma tek bir coin fırlatma efektini başlatır.
    /// </summary>
    private void SpawnCoinEffect()
    {
        if (coinFxPrefab == null)
        {
            // Eğer prefab atanmamışsa uyarı ver ve işlemi durdur.
            Debug.LogWarning("DragonController'a CoinFX Prefab'ı atanmamış.", this.gameObject);
            return;
        }

        // 1. Rastgele bir hedef nokta belirle.
        Vector2 randomCirclePoint = Random.insideUnitCircle * coinSpawnRadius;
        Vector3 targetPosition = transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0);

        // 2. Coin prefab'ını ejderhanın konumunda yarat.
        GameObject coinInstance = Instantiate(coinFxPrefab, transform.position, Quaternion.identity);

        // 3. Coin'in script'ini al ve animasyonu başlat.
        CoinFX coinFxScript = coinInstance.GetComponent<CoinFX>();
        if (coinFxScript != null)
        {
            coinFxScript.Launch(targetPosition);
        }
    }
    // --- BİTTİ ---

    public void DestroyDragon()
    {
        isDying = true;
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
        }
        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}