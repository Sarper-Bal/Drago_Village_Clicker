using UnityEngine;
using DG.Tweening;

public class CoinFX : MonoBehaviour
{
    /// <summary>
    /// Bu metot, coini hedefe doğru fırlatır, bir süre bekletir ve sonra yok eder.
    /// Tüm animasyonlar tek bir kontrol altındadır.
    /// </summary>
    public void Launch(Vector3 targetPosition)
    {
        // Gerekli animasyon parametreleri
        float jumpDuration = 0.6f;
        float jumpHeight = 1.2f;
        float waitDuration = 1.5f;
        float fadeDuration = 0.3f;

        // --- TEK BİR ANA ANİMASYON ZİNCİRİ ---
        // Bu Sequence, coinin tüm hayatını başından sonuna kadar yönetir.
        Sequence mainSequence = DOTween.Sequence();

        // 1. Zıplayarak hedefe uçma animasyonu
        mainSequence.Append(transform.DOJump(targetPosition, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutCubic));

        // 2. Ekranda bekleme süresi
        mainSequence.AppendInterval(waitDuration);

        // 3. Solup küçülerek yok olma animasyonu
        mainSequence.Append(transform.DOScale(0f, fadeDuration).SetEase(Ease.InBack));
        mainSequence.Join(GetComponent<SpriteRenderer>().DOFade(0f, fadeDuration));

        // 4. KESİN YOK ETME: Tüm animasyon zinciri bittiğinde, bu GameObject'i sahneden sil.
        // Bu OnComplete, zincirin en sonunda olduğu için her zaman çalışır.
        mainSequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}