using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<WarriorCard> SparthaCardList = new();
    public static List<WarriorCard> PersaCardList = new();

    private void Awake()
    {
        SparthaCardList = Resources.LoadAll<WarriorCard>("ScriptableObjects/Cards/Spartha").ToList();
        PersaCardList = Resources.LoadAll<WarriorCard>("ScriptableObjects/Cards/Persa").ToList();
    }
}
