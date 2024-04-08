using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayMissionCard : DisplayCard<MissionCard>, IPointerClickHandler
{
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI Points;
    public GameObject selectedBorder = null;

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

            if (selectedBorder != null)
            {
                selectedBorder.SetActive(IsSelected);
            }
        }
    }
}
