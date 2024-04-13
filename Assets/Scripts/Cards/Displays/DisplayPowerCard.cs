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
        Debug.Log(Card);
        Debug.Log(Player);
        var hasPowerCard = Player.PowerCards[Card.ExtraPoints] > 0;

        if (eventData.button == PointerEventData.InputButton.Left & hasPowerCard)
        {
            GameManager.instance.SetExtraPower(Card.ExtraPoints);
        }
    }

}
