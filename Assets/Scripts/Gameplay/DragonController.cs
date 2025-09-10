using UnityEngine;
using DG.Tweening; // DOTween kütüphanesini kullanabilmek için.

// Bu script'in çalışması için bir Collider2D bileşeni zorunludur.
[RequireComponent(typeof(Collider2D))]
public class DragonController : MonoBehaviour
{
    // Bu ejderhanın özelliklerini tutan veri.
    // Bu, Spawner tarafından atanacak.
    public DragonData dragonData;

    private Vector3 initialScale; // Animasyon için başlangıç boyutunu saklar.

    void Start()
    {
        // Başlangıç boyutunu kaydet.
        initialScale = transform.localScale;

        // dragonData'nın atanıp atanmadığını kontrol edelim. Atanmamışsa hata verir.
        if (dragonData == null)
        {
            Debug.LogError("DragonController'a DragonData atanmamış! Lütfen Spawner'ı kontrol edin.", this.gameObject);
        }
    }

    // Fare ile bu objeye tıklandığı an çalışır.
    private void OnMouseDown()
    {
        // Eğer veri atanmamışsa hiçbir şey yapma.
        if (dragonData == null) return;

        // Tıklama animasyonunu oynat.
        transform.DOPunchScale(initialScale * 0.1f, 0.2f);

        // GameManager'a kazanılan altın ve tıklama miktarını bildir.
        GameManager.Instance.AddCoins(dragonData.goldPerPress);
        GameManager.Instance.AddClicks(dragonData.clicksPerPress);
    }
}