using UnityEngine;
using UnityEngine.EventSystems; // Tıklamayı algılamak için IPointerClickHandler arayüzü.
using System.Collections.Generic; // Liste kullanmak için.

// Bu script'in çalışması için bir Collider2D bileşeni zorunludur.
[RequireComponent(typeof(Collider2D))]
public class EggShopBuilding : MonoBehaviour, IPointerClickHandler
{
    [Header("Dükkan İçeriği")]
    [Tooltip("Bu binanın satabileceği ejderha yumurtalarının listesi.")]
    [SerializeField] private List<DragonEggData> eggsForSale;

    /// <summary>
    /// Bu binanın üzerine tıklandığında EventSystem tarafından otomatik olarak çağrılır.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Eğer satılacak yumurta yoksa bir şey yapma.
        if (eggsForSale == null || eggsForSale.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} için satılacak yumurta atanmamış.");
            return;
        }

        // TODO: Bir sonraki adımda oluşturacağımız EggShopManager'a haber ver.
        // EggShopManager.Instance.ShowShopPopup(eggsForSale);
        Debug.Log($"{gameObject.name} tıklandı. Ejderha Yumurtası Dükkanı açılacak.");
    }
}