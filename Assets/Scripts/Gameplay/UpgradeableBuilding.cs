using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class UpgradeableBuilding : MonoBehaviour, IPointerClickHandler
{
    [Header("Geliştirme Zinciri")]
    [SerializeField] private UpgradeData initialUpgradeData;

    public UpgradeData CurrentUpgradeData { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    private Vector3 initialScale;

    private void Awake()
    {
        CurrentUpgradeData = initialUpgradeData;
        CurrentLevel = 1;
        initialScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentUpgradeData == null)
        {
            Debug.Log($"{gameObject.name} maksimum seviyeye ulaşmış.");
            return;
        }

        int playerLevel = GameManager.Instance.GetCurrentDragonLevel() + 1;
        if (playerLevel >= CurrentUpgradeData.minPlayerLevel)
        {
            UpgradePopupManager.Instance.ShowUpgradePopup(this);
        }
        else
        {
            Debug.Log($"'{CurrentUpgradeData.upgradeName}' için yeterli seviyede değilsin. Gerekli Seviye: {CurrentUpgradeData.minPlayerLevel}, Mevcut Seviye: {playerLevel}");
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

    // --- YENİ EKLENEN METOT: Bu metot dışarıdan çağrılarak animasyonu tetikleyecek. ---
    public void PlayActiveAnimation()
    {
        transform.DOKill();
        transform.localScale = initialScale;

        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(initialScale.x * 1.1f, initialScale.y * 0.9f, initialScale.z), 0.2f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(new Vector3(initialScale.x * 0.9f, initialScale.y * 1.1f, initialScale.z), 0.3f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(initialScale, 0.2f).SetEase(Ease.OutSine));
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}