using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<WarriorCard> SparthaCardList = new();
    public static List<WarriorCard> PersaCardList = new();

    public static List<TerrainCard> TerrainCardsList = new();

    private void Awake()
    {
        SparthaCardList = Resources.LoadAll<WarriorCard>("ScriptableObjects/Cards/Warriors/Spartha").ToList();
        PersaCardList = Resources.LoadAll<WarriorCard>("ScriptableObjects/Cards/Warriors/Persa").ToList();
        TerrainCardsList = Resources.LoadAll<TerrainCard>("ScriptableObjects/Cards/Terrains/").ToList();
    }
}
