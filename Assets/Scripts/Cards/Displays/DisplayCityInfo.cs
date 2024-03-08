using TMPro;
using UnityEngine;

public class DisplayCityInfo : MonoBehaviour
{
    public City City;
    public TextMeshProUGUI CityNameText;
    public TextMeshProUGUI RiverQuantityText;
    public TextMeshProUGUI DesertQuantityText;
    public TextMeshProUGUI PlainsQuantityText;
    public TextMeshProUGUI MountainsQuantityText;
    public TextMeshProUGUI AdvantageText;

    protected void Start()
    {
        CityNameText.text = City.name;
        RiverQuantityText.text = City.GetRiverQuantity().ToString();
        DesertQuantityText.text = City.GetDesertQuantity().ToString();
        PlainsQuantityText.text = City.GetPlainsQuantity().ToString();
        MountainsQuantityText.text = City.GetMountainsQuantity().ToString();
        AdvantageText.text = "PlaceHolder";
    }

}
