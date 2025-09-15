using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        // EggShopManager'a haber ver ve satılacak yumurtaların listesini gönder.
        EggShopManager.Instance.ShowShopPopup(eggsForSale);
    }
}