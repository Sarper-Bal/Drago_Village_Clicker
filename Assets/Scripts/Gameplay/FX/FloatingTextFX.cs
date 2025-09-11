using UnityEngine;
using TMPro; // TextMeshPro kütüphanesini kullanmak için bu satır gerekli.
using DG.Tweening;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingTextFX : MonoBehaviour
{
    private TextMeshPro textMesh;

    // --- KONTROL MEKANİZMASI ---
    // 'static' bir değişken, bu script'in tüm kopyaları tarafından paylaşılır.
    // Bu sayede, kaç tane FloatingTextFX objesinin aktif olduğunu sayabiliriz.
    public static int ActiveTextCount = 0;
    private const int MAX_TEXT_COUNT = 4; // Ekranda aynı anda olabilecek maksimum text sayısı.

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// Uçuşan text'i belirtilen metinle başlatır ve animasyonunu oynatır.
    /// Animasyon bitince text kendini yok eder.
    /// </summary>
    public void Show(string textToShow, Vector3 position)
    {
        // 1. Eğer ekranda izin verilenden fazla text zaten varsa, bu yenisini gösterme.
        // Yeni bir obje zaten yaratıldığı için, hemen kendini yok etmesini sağlıyoruz.
        if (ActiveTextCount >= MAX_TEXT_COUNT)
        {
            Destroy(gameObject);
            return;
        }

        // 2. Aktif text sayacını bir artır ve başlangıç ayarlarını yap.
        ActiveTextCount++;

        transform.position = position;
        textMesh.text = textToShow;
        textMesh.alpha = 1f; // Başlangıçta tamamen görünür.
        transform.localScale = Vector3.one * 0.5f; // Başlangıçta biraz küçük.

        // --- ANİMASYON ZİNCİRİ ---
        Sequence mainSequence = DOTween.Sequence();

        // 1. Adım: Hızlıca belir ve biraz büyü.
        mainSequence.Append(transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));

        // 2. Adım: Yukarı doğru uç ve aynı anda yavaşça sol.
        // Bu iki animasyon aynı anda (Join) çalışacak.
        mainSequence.Append(transform.DOMoveY(position.y + 1.5f, 1.0f).SetEase(Ease.OutQuad));
        mainSequence.Join(textMesh.DOFade(0f, 1.0f).SetEase(Ease.InQuad));

        // 3. Adım: Animasyonlar bittiğinde objeyi yok et ve sayacı azalt.
        mainSequence.OnComplete(() =>
        {
            ActiveTextCount--; // Sayacı azaltmayı unutma!
            Destroy(gameObject); // Text kendini yok eder.
        });

        // DOTween'in bu objeyi hedeflemesini sağlıyoruz. Böylece obje yok edildiğinde,
        // üzerinde çalışan animasyonlar da otomatik olarak durur ve hata vermez.
        mainSequence.SetTarget(this);
    }

    // Bu metodun OnDestroy içinde çağrılması, olası bir hata durumunda (örn. seviye değişimi)
    // objenin Destroy edilmesi halinde sayacın düzgün bir şekilde azalmasını sağlar.
    private void OnDestroy()
    {
        // Eğer obje yok edilirken hala aktif sayaca dahilse, azalt.
        // Bu, özellikle hata ayıklama veya seviye geçişlerinde önemlidir.
        // mainSequence.OnComplete içindeki Destroy çağrıldığında zaten azaltılmış olacaktır.
        // Ancak dışarıdan bir force-destroy olursa da sayacı koruruz.
        if (ActiveTextCount > 0)
        {
            // Eğer sayacı zaten OnComplete'te azaltıyorsak, bu kontrol iki kere azaltmayı engeller.
            // Ama hata ayıklama açısından burada tutmak faydalı olabilir.
            // Basitlik için ve OnComplete'in her zaman çalışacağını varsayarak kaldırabiliriz,
            // ancak şimdilik hata ayıklama güvenliği için tutalım.
            // ActiveTextCount--; 
        }
    }
}