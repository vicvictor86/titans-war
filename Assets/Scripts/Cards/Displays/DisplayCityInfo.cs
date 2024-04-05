using TMPro;
using UnityEngine;

public class DisplayCityInfo : MonoBehaviour
{
    public City City;
    public TextMeshProUGUI CityNameText;
    public TextMeshProUGUI AdvantageText;
    public TextMeshProUGUI Player;

    protected void Start()
    {
        CityNameText.text = City.name;
        AdvantageText.text = City.GetBenefitDescription();
        //Deve ser trocado para nome do jogador ou coisa do tipo
        Player.text = City.Owner?.PlayerSide ?? "Livre";
    }

}
