using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<WarriorCard> cardsList = new();

    private void Awake()
    {
        cardsList = Resources.LoadAll<WarriorCard>("ScriptableObjects/Cards").ToList();
    }
}
