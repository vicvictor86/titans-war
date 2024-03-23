using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayPowerCard : DisplayCard<PowerCard>, IPointerClickHandler
{
    public PlayerDeck Player;
    public TextMeshProUGUI Points;

    protected override void Start()
    {
        base.Start();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.instance.SetExtraPower(Card.ExtraPoints);
        }
    }

}
