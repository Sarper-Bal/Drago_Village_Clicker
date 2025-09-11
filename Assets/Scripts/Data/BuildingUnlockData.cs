using UnityEngine;

// [System.Serializable] özelliği, bu sınıfın Unity Inspector'da görünebilmesini sağlar.
[System.Serializable]
public class BuildingUnlockData
{
    [Tooltip("İnşa edilecek binanın prefab'ı.")]
    public GameObject buildingPrefab;

    [Tooltip("Bu binanın inşa edileceği spawn noktasının benzersiz kimliği (ID). Sahnedeki nokta ile eşleşmeli.")]
    public string spawnPointID;
}