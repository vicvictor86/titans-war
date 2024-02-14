using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")] 
public class Card : ScriptableObject
{
    public int Id;
    public string CardName;
    public Sprite CardImage;
}
