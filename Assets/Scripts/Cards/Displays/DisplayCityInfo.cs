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
    public TextMeshProUGUI Player;

    protected void Start()
    {
        CityNameText.text = City.name;
        RiverQuantityText.text = City.GetRiverQuantity().ToString();
        DesertQuantityText.text = City.GetDesertQuantity().ToString();
        PlainsQuantityText.text = City.GetPlainsQuantity().ToString();
        MountainsQuantityText.text = City.GetMountainsQuantity().ToString();
        AdvantageText.text = City.GetBenefitDescription();
        //Deve ser trocado para nome do jogador ou coisa do tipo
        Player.text = (GameManager.instance.playerList.IndexOf(City.Owner) + 1).ToString();
    }

}
