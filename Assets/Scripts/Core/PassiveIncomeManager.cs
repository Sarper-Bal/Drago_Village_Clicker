using UnityEngine;

public class PassiveIncomeManager : Singleton<PassiveIncomeManager>
{
    // Oyuncunun tüm geliştirmelerden kazandığı toplam saniye başına altın miktarı.
    private int totalGoldPerSecond = 0;

    // Zamanı takip etmek için bir sayaç.
    private float timer = 0f;

    // Update metodu her frame'de çalışır.
    void Update()
    {
        // Eğer hiç pasif kazanç yoksa, boşuna işlem yapma. Bu küçük bir optimizasyondur.
        if (totalGoldPerSecond == 0) return;

        // Geçen süreyi sayaca ekle.
        timer += Time.deltaTime;

        // Eğer sayaç 1 saniyeyi geçtiyse...
        if (timer >= 1f)
        {
            // ...GameManager'a toplam saniye başına altın miktarını ekle.
            GameManager.Instance.AddCoins(totalGoldPerSecond);
            // ...sayacı sıfırla (kalan küsuratı koruyarak daha hassas bir zamanlama sağlar).
            timer -= 1f;
        }
    }

    /// <summary>
    /// Toplam saniye başına altın kazancına yeni bir bonus ekler.
    /// ShopManager tarafından çağrılacak.
    /// </summary>
    /// <param name="amount">Eklenecek bonus miktarı.</param>
    public void AddGoldPerSecond(int amount)
    {
        totalGoldPerSecond += amount;
    }
}
