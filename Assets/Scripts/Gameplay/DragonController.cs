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

    [Header("Coin Fırlatma Efekti")]
    [Tooltip("Fırlatılacak olan görsel coin prefab'ı.")]
    [SerializeField] private GameObject coinFxPrefab;
    [Tooltip("Coin'in fırlatılacağı alanın yarıçapı.")]
    [SerializeField] private float coinSpawnRadius = 2f;

    [Header("Uçuşan Text Efekti")]
    [Tooltip("Kazancı göstermek için fırlatılacak olan text prefab'ı.")]
    [SerializeField] private GameObject floatingTextFxPrefab;

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
        int goldGained = dragonData.goldPerPress;

        GameManager.Instance.AddCoins(goldGained);
        GameManager.Instance.AddClicks(dragonData.clicksPerPress);

        SpawnCoinEffect();
        ShowFloatingTextEffect(goldGained);

        Debug.Log(gameObject.name + " tıklandı!");
    }

    private void SpawnCoinEffect()
    {
        if (coinFxPrefab == null) return;
        Vector2 randomCirclePoint = Random.insideUnitCircle * coinSpawnRadius;
        Vector3 targetPosition = transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0);
        GameObject coinInstance = Instantiate(coinFxPrefab, transform.position, Quaternion.identity);
        CoinFX coinFxScript = coinInstance.GetComponent<CoinFX>();
        if (coinFxScript != null)
        {
            coinFxScript.Launch(targetPosition);
        }
    }

    private void ShowFloatingTextEffect(int goldAmount)
    {
        if (floatingTextFxPrefab == null)
        {
            Debug.LogWarning("DragonController'a FloatingTextFX Prefab'ı atanmamış.", this.gameObject);
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
        GameObject textInstance = Instantiate(floatingTextFxPrefab, spawnPosition, Quaternion.identity);

        FloatingTextFX textFxScript = textInstance.GetComponent<FloatingTextFX>();
        if (textFxScript != null)
        {
            textFxScript.Show("+" + goldAmount.ToString(), spawnPosition);
        }
    }

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