using UnityEngine;
using System.Collections.Generic; // List kullanmak için bu kütüphane gerekli.

[CreateAssetMenu(fileName = "NewVillageData", menuName = "Game/Village Data")]
public class VillageData : ScriptableObject
{
    [Header("Köy Kimliği")]
    [Tooltip("Köyün benzersiz kimliği. Kayıt sisteminde kullanılacak.")]
    public string villageID;
    [Tooltip("Arayüzde görünecek köy adı.")]
    public string villageName = "Yeni Köy";

    [Header("Sahne Bilgisi")]
    [Tooltip("Bu köy için yüklenecek olan sahnenin tam adı.")]
    public string sceneToLoad;

    [Header("Köy İçeriği")]
    [Tooltip("Bu köydeki ejderhanın seviye ilerlemesini tanımlayan LevelData listesi.")]
    public List<LevelData> villageLevelProgression;
    // TODO: Gelecekte bu köye özel binalar veya geliştirmeler buraya eklenebilir.
}