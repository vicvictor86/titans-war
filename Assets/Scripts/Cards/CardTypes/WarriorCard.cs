using Domain;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "WarriorCard")]
[Serializable]
public class WarriorCard : Card
{
    public int TotalForce => WaterForce + DesertForce + MountainsForce + PlainsForce;

    public int WaterForce;
    public int DesertForce;
    public int MountainsForce;
    public int PlainsForce;
    public string Nationality;

    public int WaterValue => TotalForce + WaterForce;
    public int DesertValue => TotalForce + DesertForce;
    public int MountainsValue => TotalForce + MountainsForce;
    public int PlainsValue => TotalForce + PlainsForce;

    public int GetPowerValue(TerrainType type)
    {
        return type switch
        {
            TerrainType.RIVER => WaterValue,
            TerrainType.DESERT => DesertValue,
            TerrainType.MOUNTAINS => MountainsValue,
            TerrainType.PLAINS => PlainsValue,
            _ => 0
        };
    }

}
