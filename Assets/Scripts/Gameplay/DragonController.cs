using UnityEngine;
using UnityEngine.EventSystems; // Event System'i kullanmak için bu satır ÇOK ÖNEMLİ!
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
// IPointerClickHandler arayüzünü ekliyoruz. Bu, objenin tıklanabilir olduğunu sisteme bildirir.
public class DragonController : MonoBehaviour, IPointerClickHandler
{
    public DragonData dragonData;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
        if (dragonData == null)
        {
            Debug.LogError("DragonController'a DragonData atanmamış! Lütfen Spawner'ı kontrol edin.", this.gameObject);
        }
    }

    // OnMouseDown yerine bu fonksiyonu kullanacağız.
    // Bu fonksiyon, EventSystem tarafından collider'a sahip bu objeye tıklandığında OTOMATİK olarak çağrılır.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragonData == null) return;

        // Tıklama animasyonu
        transform.DOPunchScale(initialScale * 0.1f, 0.2f);

        // GameManager'a kazanılan altın ve tıklama miktarını bildir.
        GameManager.Instance.AddCoins(dragonData.goldPerPress);
        GameManager.Instance.AddClicks(dragonData.clicksPerPress);

        // Konsola test mesajı ekleyelim, çalıştığından emin olalım.
        Debug.Log(gameObject.name + " tıklandı!");
    }
}