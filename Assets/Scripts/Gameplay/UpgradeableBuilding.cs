using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class UpgradeableBuilding : MonoBehaviour, IPointerClickHandler
{
    [Header("Geliştirme Zinciri")]
    [SerializeField] private UpgradeData initialUpgradeData;

    [Header("Görsel Efektler")]
    [SerializeField] private GameObject floatingTextFxPrefab;
    [SerializeField] private float textSpawnOffsetY = 1.0f;

    public UpgradeData CurrentUpgradeData { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    private int currentGoldPerSecond = 0;
    // 'timer' ve 'Update' metodu kaldırıldı.

    private Sequence activeAnimationSequence;
    private Vector3 initialScale;

    private void Awake()
    {
        CurrentUpgradeData = initialUpgradeData;
        CurrentLevel = 1;
        initialScale = transform.localScale;
    }

    // --- Update() METODU TAMAMEN KALDIRILDI ---

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentUpgradeData == null)
        {
            Debug.Log($"{gameObject.name} maksimum seviyeye ulaşmış.");
            return;
        }

        int playerLevel = GameManager.Instance.CurrentLevelIndex + 1;
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
        currentGoldPerSecond += CurrentUpgradeData.goldPerSecondBonus;
        StartActiveAnimation(); // Animasyonu ve gelir döngüsünü başlat/güncelle.

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

    /// <summary>
    /// Binanın sürekli "canlılık" animasyonunu ve senkronize gelir üretimini başlatır.
    /// </summary>
    private void StartActiveAnimation()
    {
        if (activeAnimationSequence != null && activeAnimationSequence.IsActive())
        {
            activeAnimationSequence.Kill();
        }
        transform.localScale = initialScale;

        activeAnimationSequence = DOTween.Sequence();

        // Animasyon zincirini oluşturuyoruz.
        activeAnimationSequence.Append(transform.DOScale(new Vector3(initialScale.x * 1.1f, initialScale.y * 0.9f, initialScale.z), 0.6f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(new Vector3(initialScale.x * 0.9f, initialScale.y * 1.1f, initialScale.z), 0.6f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(initialScale, 0.5f).SetEase(Ease.InOutSine))
            .AppendInterval(0.3f) // Animasyonlar bittikten sonra kısa bir bekleme. Toplam süre yaklaşık 2 saniye.

            // --- YENİ VE GÜNCELLENMİŞ MANTIK ---
            // 'onStepComplete', her döngü tamamlandığında bu kod bloğunu çalıştırır.
            .OnStepComplete(() =>
            {
                // Eğer bina hala gelir üretiyorsa...
                if (currentGoldPerSecond > 0)
                {
                    // ...altını ekle ve kazanç metnini göster!
                    GameManager.Instance.AddCoins(currentGoldPerSecond);
                    ShowIncomeFeedback(currentGoldPerSecond);
                }
            })
            .SetLoops(-1); // Sonsuz döngü.
    }

    private void ShowIncomeFeedback(int goldAmount)
    {
        if (floatingTextFxPrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * textSpawnOffsetY;
        GameObject textInstance = Instantiate(floatingTextFxPrefab, spawnPosition, Quaternion.identity);

        FloatingTextFX textFxScript = textInstance.GetComponent<FloatingTextFX>();
        if (textFxScript != null)
        {
            textFxScript.Show("+" + goldAmount.ToString(), spawnPosition);
        }
    }

    private void OnDestroy()
    {
        if (activeAnimationSequence != null && activeAnimationSequence.IsActive())
        {
            activeAnimationSequence.Kill();
        }
    }
}