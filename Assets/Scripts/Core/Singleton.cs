using UnityEngine;

/// <summary>
/// Singleton deseni için genel bir ana sınıf.
/// Bu sınıftan türeyen herhangi bir sınıf (T), oyun sahnesinde tek bir örnek olarak var olacaktır.
/// </summary>
/// <typeparam name="T">Singleton yapılacak sınıfın türü.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Sahnedeki bu türden bir nesne bulmaya çalış. (Unity'nin yeni versiyonuyla güncellendi)
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }
}