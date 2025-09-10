using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween'i ve Sequence özelliğini kullanmak için bu satır önemli.

[RequireComponent(typeof(Collider2D))]
public class DragonController : MonoBehaviour, IPointerClickHandler
{
    public DragonData dragonData;
    private Vector3 initialScale;

    // Animasyon ayarlarını Inspector'dan kolayca değiştirebilmek için değişkenler ekliyoruz.
    [Header("Tıklama Animasyon Ayarları")]
    [SerializeField] private float clickAnimationDuration = 0.2f; // Animasyonun toplam süresi.
    [SerializeField] private float scaleMultiplier = 1.15f; // Tıklandığında ne kadar büyüyeceği.
    [SerializeField] private float shakeStrength = 0.1f; // Sarsılma ne kadar güçlü olacak.

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
        if (dragonData == null) return;

        // 1. Ana tıklama fonksiyonu artık sadece animasyonu ve oyun mantığını tetikliyor.
        // Bu, kodun okunabilirliğini artırır.
        PlayClickFeedbackAnimation();
        ProcessGameLogic();
    }

    /// <summary>
    /// Tıklama anında çalışacak olan tüm görsel geri bildirim animasyonlarını yönetir.
    /// </summary>
    private void PlayClickFeedbackAnimation()
    {
        // Optimizasyon: Eğer bu obje üzerinde çalışan bir animasyon varsa, onu anında bitir.
        // Bu, oyuncu çok hızlı tıkladığında animasyonların çakışmasını ve garip görünmesini engeller.
        transform.DOComplete();

        // DOTween Sequence, birden fazla animasyonu birleştirmemizi ve aynı anda oynatmamızı sağlar.
        // Bu, daha zengin ve katmanlı animasyonlar için mükemmel bir yöntemdir.
        DOTween.Sequence()
            // Append: Sıraya yeni bir animasyon ekler.
            // DOPunchScale: Objeyi belirtilen miktarda anlık büyütür/küçültür ve eski haline döndürür.
            // Bu, "vuruş" hissinin temelini oluşturur.
            .Append(transform.DOPunchScale(initialScale * (scaleMultiplier - 1), clickAnimationDuration, 1, 0.5f))

            // Join: Sıradaki bir önceki animasyonla AYNI ANDA başlayacak yeni bir animasyon ekler.
            // DOShakePosition: Objeyi belirtilen süre ve güçte sarsar.
            // Bu, vuruşun "etkisini" ve gücünü artırır.
            .Join(transform.DOShakePosition(clickAnimationDuration, new Vector3(shakeStrength, shakeStrength, 0), 10, 90, false, true));
    }

    /// <summary>
    /// Tıklama sonrası çalışacak olan oyun mantığını (altın ekleme vb.) yönetir.
    /// </summary>
    private void ProcessGameLogic()
    {
        // GameManager'a kazanılan altın ve tıklama miktarını bildir.
        GameManager.Instance.AddCoins(dragonData.goldPerPress);
        GameManager.Instance.AddClicks(dragonData.clicksPerPress);

        // Konsola test mesajı.
        Debug.Log(gameObject.name + " tıklandı!");
    }
}