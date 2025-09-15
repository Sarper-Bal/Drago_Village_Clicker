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
    private float timer = 0f;
    private Vector3 initialScale;

    private void Awake()
    {
        CurrentUpgradeData = initialUpgradeData;
        CurrentLevel = 1;
        initialScale = transform.localScale;
    }

    /// <summary>
    /// Her saniye pasif altın üretir ve SENKRONİZE bir şekilde görsel efektleri tetikler.
    /// </summary>
    private void Update()
    {
        // Eğer bina pasif altın üretmiyorsa, bu metodu çalıştırma.
        if (currentGoldPerSecond == 0) return;

        // Zamanlayıcıyı artır.
        timer += Time.deltaTime;

        // Her 1 saniyede bir...
        if (timer >= 1f)
        {
            // 1. Altını ekle.
            GameManager.Instance.AddCoins(currentGoldPerSecond);
            // 2. Kazanç metnini göster.
            ShowIncomeFeedback(currentGoldPerSecond);
            // 3. Jöle animasyonunun BİR döngüsünü oynat.
            PlayActiveAnimation();

            // Zamanlayıcıyı sıfırla.
            timer -= 1f;
        }
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
        // Geliştirme yapıldığında, binanın saniye başına gelirini artır.
        // Bu, Update metodundaki döngüyü otomatik olarak başlatacak.
        currentGoldPerSecond += CurrentUpgradeData.goldPerSecondBonus;

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
    /// Binanın yanlardan sıkışıp uzadığı "canlılık" animasyonunun TEK BİR döngüsünü oynatır.
    /// </summary>
    private void PlayActiveAnimation()
    {
        // Önceki animasyonların çakışmaması için çalışanları durdur ve boyutu sıfırla.
        transform.DOKill();
        transform.localScale = initialScale;

        // Yeni bir animasyon zinciri oluştur.
        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(initialScale.x * 1.1f, initialScale.y * 0.9f, initialScale.z), 0.2f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(new Vector3(initialScale.x * 0.9f, initialScale.y * 1.1f, initialScale.z), 0.3f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(initialScale, 0.2f).SetEase(Ease.OutSine));
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

    // OnDestroy metodu artık sürekli bir animasyonu durdurmak zorunda olmadığı için kaldırılabilir veya
    // gelecekteki olası temizlik işlemleri için boş bırakılabilir. Şimdilik temizlik için bırakıyoruz.
    private void OnDestroy()
    {
        transform.DOKill();
    }
}