using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")] 
public class Card : ScriptableObject
{
    public int id;
    public string cardName;
    public Sprite cardImage;
    public string type;

    public int totalForce;
    public int waterForce;
    public int desertForce;
    public int mountainsForce;
    public int plainsForce;

}
