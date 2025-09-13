using UnityEngine;

public class PassiveIncomeManager : Singleton<PassiveIncomeManager>
{
    private int totalGoldPerSecond = 0;
    private float timer = 0f;

    void Update()
    {
        if (totalGoldPerSecond == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            GameManager.Instance.AddCoins(totalGoldPerSecond);
            timer -= 1f;
        }
    }

    /// <summary>
    /// Toplam saniye başına altın kazancına yeni bir bonus ekler.
    /// </summary>
    public void AddGoldPerSecond(int amount)
    {
        totalGoldPerSecond += amount;
        Debug.Log($"Saniye Başına Kazanç {amount} arttı. Yeni Değer: {totalGoldPerSecond}");
    }
}