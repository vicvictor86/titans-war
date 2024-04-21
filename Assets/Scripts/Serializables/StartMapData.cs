using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StartMap 
{
    public List<StartTerritoryData> startTerritoriesData = new();
}

public class StartTerritoryData
{
    public float posx;
    public float posy;
    public string CityName;
    public int Point;
    public TerrainType Type;
}
