using Newtonsoft.Json;
using System;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")]
[Serializable]
public class Card : ScriptableObject
{
    [SerializeField] public string CardName;
    [JsonIgnore] public Sprite CardImage;
}
