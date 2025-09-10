using UnityEngine;

/// <summary>
/// Singleton deseni için genel bir ana sınıf.
/// Bu sınıftan türeyen herhangi bir sınıf (T), oyun sahnesinde tek bir örnek olarak var olacaktır.
/// </summary>
/// <typeparam name="T">Singleton yapılacak sınıfın türü.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    // Bu, sınıfın tek örneğini (instance) tutacak olan özel değişkendir.
    private static T _instance;

    // Bu, sınıfın tek örneğine dışarıdan erişim sağlayan genel özelliktir (property).
    public static T Instance
    {
        get
        {
            // Eğer _instance henüz oluşturulmamışsa...
            if (_instance == null)
            {
                // Sahnedeki bu türden bir nesne bulmaya çalış.
                _instance = FindObjectOfType<T>();

                // Eğer sahnede bu türden bir nesne bulunamazsa...
                if (_instance == null)
                {
                    // Yeni bir GameObject oluştur ve ona bu bileşeni (component) ekle.
                    GameObject obj = new GameObject();
                    // Oluşturulan nesnenin adını, bileşenin adıyla aynı yap (kolay tanımak için).
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            // Son olarak, bulunan veya oluşturulan örneği döndür.
            return _instance;
        }
    }

    /// <summary>
    /// Awake metodu, Unity tarafından nesne oluşturulduğunda ilk çağrılan fonksiyondur.
    /// </summary>
    public virtual void Awake()
    {
        // Eğer _instance henüz atanmamışsa...
        if (_instance == null)
        {
            // Bu nesneyi tek örnek olarak ata.
            _instance = this as T;
            // Bu nesnenin, sahneler arasında geçiş yapıldığında yok olmamasını sağla.
            // Bu, GameManager gibi yöneticilerin oyun boyunca varlığını sürdürmesi için önemlidir.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // Eğer sahnede zaten bir _instance varsa ve o bu nesne değilse,
            // bu yeni oluşturulan nesneyi yok et. Bu, birden fazla örneğin oluşmasını engeller.
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }
}