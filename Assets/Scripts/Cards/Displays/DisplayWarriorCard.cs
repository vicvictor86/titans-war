using TMPro;

public class DisplayWarriorCard : DisplayCard<WarriorCard>
{
    public TextMeshProUGUI TypeText;
    public TextMeshProUGUI TotalForceText;
    public TextMeshProUGUI WaterForceText;
    public TextMeshProUGUI DesertForceText;
    public TextMeshProUGUI MountainsForceText;
    public TextMeshProUGUI PlainsForceText;

    protected override void Start()
    {
        base.Start();
        TypeText.text = Card.Nationality;
        TotalForceText.text = Card.TotalForce.ToString();
        WaterForceText.text = Card.WaterForce.ToString();
        DesertForceText.text = Card.DesertForce.ToString();
        MountainsForceText.text = Card.MountainsForce.ToString();
        PlainsForceText.text = Card.PlainsForce.ToString();
    }
}
