using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCardDeck
{
    public static int totalQuantity = 0;

    public TerrainCard terrainCard;
    public int quantityCard;

    public TerrainCardDeck(TerrainCard terrainCard, int quantityCard)
    {
        this.terrainCard = terrainCard;
        this.quantityCard = quantityCard;
        totalQuantity += quantityCard;
    }

    public static TerrainType SelectRandomTerrainType()
    {
        Array values = Enum.GetValues(typeof(TerrainType));
        
        int randomEnumNumber = UnityEngine.Random.Range(0, values.Length);

        TerrainType terrainSelected = (TerrainType) values.GetValue(randomEnumNumber);

        return terrainSelected;
    }
}
