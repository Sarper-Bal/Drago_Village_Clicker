using UnityEngine;
using System.Collections.Generic;

// Bu script, bir köyün tüm verilerini bir arada tutar.
[CreateAssetMenu(fileName = "NewVillageData", menuName = "Game/Village Data")]
public class VillageData : ScriptableObject
{
    [Header("Köy Kimliği")]
    public string villageID = "default_village"; // Kayıt ve referans için benzersiz kimlik.
    public string villageName = "Ejderha Köyü"; // Oyuncuya gösterilecek isim.
    public string sceneToLoad; // Bu köye geçildiğinde yüklenecek sahnenin adı.

    [Header("Köy İlerleme Hedefi")]
    [Tooltip("Bu köyü tamamlamak ve bir sonrakine geçmek için ulaşılması gereken altın miktarı.")]
    public int goldToCompleteVillage = 1000; // YENİ EKLENEN ALAN

    [Header("Seviye İlerlemesi")]
    [Tooltip("Bu köydeki ejderha seviyeleri ve seviye atlama koşulları.")]
    public List<LevelData> villageLevelProgression;
}