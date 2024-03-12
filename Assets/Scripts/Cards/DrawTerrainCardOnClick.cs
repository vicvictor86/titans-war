using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawTerrainCardOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerDeck playerDeck;

    public void OnPointerClick(PointerEventData eventData)
    {
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        bool canDrawCard = GameManager.instance.CanDrawCard(playerDeck);
        if (clickWithLeftButton && canDrawCard && playerDeck != null)
        {
            var drawedCard = playerDeck.DrawTerrainCard();

            if (drawedCard is not null)
            {
                GameManager.instance.actionMade = true;
            }
        }
    }
}
