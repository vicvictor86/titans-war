using TMPro;

public class DisplayMissionCard : DisplayCard<MissionCard>
{
    public TextMeshProUGUI DescriptionText;

    protected override void Start()
    {
        base.Start();
        DescriptionText.text = Card.Description;
    }
}
