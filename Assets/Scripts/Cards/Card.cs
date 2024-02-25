using System;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")] 
public class Card : ScriptableObject
{
    public string CardName;
    public Sprite CardImage;
}
