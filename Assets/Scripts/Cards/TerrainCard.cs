using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "TerrainCard")]
public class TerrainCard : Card
{
    public enum TerrainType
    {
        WATER,
        PLAINS,
        DESERT,
        MOUNTAINS
    }

    public TerrainType Type;
    public int Strength;
}