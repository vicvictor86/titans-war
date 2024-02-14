using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "WarriorCard")]
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

}
