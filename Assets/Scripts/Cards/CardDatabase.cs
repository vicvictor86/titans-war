using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardsList = new();

    private void Awake()
    {
        cardsList = Resources.LoadAll<Card>("ScriptableObjects/Cards").ToList();
    }
}
