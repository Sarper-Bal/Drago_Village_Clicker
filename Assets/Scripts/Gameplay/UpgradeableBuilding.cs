using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class UpgradeableBuilding : MonoBehaviour, IPointerClickHandler
{
    [Header("Geliştirme Zinciri")]
    [Tooltip("Bu binanın ilk geliştirme seviyesini temsil eden veri dosyası.")]
    [SerializeField] private UpgradeData initialUpgradeData;

    [Header("Görsel Efektler")]
    [Tooltip("Geliştirme sonrası kazancı göstermek için fırlatılacak olan text prefab'ı.")]
    [SerializeField] private GameObject floatingTextFxPrefab;
    [SerializeField] private float textSpawnOffsetY = 1.0f;

    public UpgradeData CurrentUpgradeData { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    private int currentGoldPerSecond = 0;
    private float timer = 0f;
    private Sequence activeAnimationSequence;
    private Vector3 initialScale;

    private void Awake()
    {
        CurrentUpgradeData = initialUpgradeData;
        CurrentLevel = 1;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (currentGoldPerSecond == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            GameManager.Instance.AddCoins(currentGoldPerSecond);
            ShowIncomeFeedback(currentGoldPerSecond);
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

        // --- DÜZELTME BURADA ---
        // GameManager'dan oyuncunun mevcut ejderha seviyesini eski değişken yerine yeni metot ile alıyoruz.
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
        currentGoldPerSecond += CurrentUpgradeData.goldPerSecondBonus;
        StartActiveAnimation();

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

    private void StartActiveAnimation()
    {
        if (activeAnimationSequence != null && activeAnimationSequence.IsActive())
        {
            activeAnimationSequence.Kill();
        }
        transform.localScale = initialScale;

        activeAnimationSequence = DOTween.Sequence();
        activeAnimationSequence.Append(transform.DOScale(new Vector3(initialScale.x * 1.1f, initialScale.y * 0.9f, initialScale.z), 0.6f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(new Vector3(initialScale.x * 0.9f, initialScale.y * 1.1f, initialScale.z), 0.6f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(initialScale, 0.5f).SetEase(Ease.InOutSine))
            .AppendInterval(1f)
            .SetLoops(-1);
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