using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayMissionCard : DisplayCard<MissionCard>, IPointerClickHandler
{
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI Points;

    public bool isClickable;
    public bool IsSelected = false;

    protected override void Start()
    {
        base.Start();
        DescriptionText.text = Card.Description;
        Points.text = Card.Points.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClickable)
        {
            return;
        }

        if (PointerEventData.InputButton.Left == eventData.button)
        {
            Debug.Log("Selecionado");
            IsSelected = !IsSelected;
        }
    }
}
