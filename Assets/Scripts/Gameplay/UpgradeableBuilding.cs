using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class UpgradeableBuilding : MonoBehaviour, IPointerClickHandler
{
    [Header("Geliştirme Zinciri")]
    [Tooltip("Bu binanın ilk geliştirme seviyesini temsil eden veri dosyası.")]
    [SerializeField] private UpgradeData initialUpgradeData;

    public UpgradeData CurrentUpgradeData { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    private void Awake()
    {
        CurrentUpgradeData = initialUpgradeData;
        CurrentLevel = 1;
    }

    /// <summary>
    /// Bu binanın üzerine tıklandığında, oyuncu seviyesini kontrol ederek paneli açar.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Eğer geliştirme zinciri bittiyse (max seviyeye ulaştıysa) bir şey yapma.
        if (CurrentUpgradeData == null)
        {
            Debug.Log($"{gameObject.name} maksimum seviyeye ulaşmış.");
            // TODO: Oyuncuya maksimum seviyede olduğuna dair bir görsel geri bildirim gösterilebilir.
            return;
        }

        // --- YENİ EKLENEN SEVİYE KONTROLÜ ---
        // GameManager'dan oyuncunun mevcut ejderha seviyesini al.
        // CurrentLevelIndex 0'dan başladığı için karşılaştırma yaparken 1 ekliyoruz.
        int playerLevel = GameManager.Instance.CurrentLevelIndex + 1;

        // Oyuncunun seviyesi, bu geliştirmeyi görmek için gereken minimum seviyeden büyük veya eşit mi?
        if (playerLevel >= CurrentUpgradeData.minPlayerLevel)
        {
            // Eğer seviye yeterliyse, geliştirme panelini göster.
            UpgradePopupManager.Instance.ShowUpgradePopup(this);
        }
        else
        {
            // Eğer seviye yeterli değilse, paneli açma ve konsola bilgi ver.
            Debug.Log($"'{CurrentUpgradeData.upgradeName}' için yeterli seviyede değilsin. Gerekli Seviye: {CurrentUpgradeData.minPlayerLevel}, Mevcut Seviye: {playerLevel}");
            // TODO: Oyuncuya "Daha yüksek seviye gerekli" diye bir geri bildirim gösterilebilir.
        }
    }

    public void AdvanceToNextUpgrade()
    {
        if (CurrentUpgradeData.nextUpgrade != null)
        {
            CurrentUpgradeData = CurrentUpgradeData.nextUpgrade;
            CurrentLevel++;
        }
        else
        {
            CurrentUpgradeData = null;
            CurrentLevel++;
        }
    }
}