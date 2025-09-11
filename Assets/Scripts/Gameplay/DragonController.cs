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

    private Sequence clickSequence;
    private bool isDying = false; // GÜVENLİK: Ejderhanın yok edilme sürecinde olup olmadığını kontrol eder.

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
        // GÜVENLİK: Eğer ejderha zaten yok ediliyorsa, yeni bir tıklama işlemi başlatma.
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
        Debug.Log(gameObject.name + " tıklandı!");
    }

    /// <summary>
    /// Bu ejderhayı animasyonlu bir şekilde güvenle yok eder. DragonSpawner tarafından çağrılır.
    /// </summary>
    public void DestroyDragon()
    {
        // 1. Yok edilme sürecini başlat ve tekrar tıklanmasını engelle.
        isDying = true;

        // 2. Üzerinde çalışan TÜM animasyonları (tıklama dahil) anında ve güvenle durdur.
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
        }
        transform.DOKill();

        // 3. Kendi yok olma animasyonunu başlat ve bittiğinde objeyi yok et.
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}