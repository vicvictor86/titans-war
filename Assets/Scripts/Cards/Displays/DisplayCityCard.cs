using TMPro;

public class DisplayCityCard : DisplayCard<CityCard>
{
    public TextMeshProUGUI DescriptionText;

    protected override void Start()
    {
        base.Start();
        DescriptionText.text = Card.CityBenefits;
    }
}
