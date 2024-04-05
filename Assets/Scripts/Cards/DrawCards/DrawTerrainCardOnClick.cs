using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawTerrainCardOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var playerDeck = GameManager.instance.ActualPlayer;
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        bool canDrawCard = GameManager.instance.CanDrawCard(playerDeck);
        if (clickWithLeftButton && canDrawCard && playerDeck != null && playerDeck.WarriorCardsInPlayerHand.Any())
        {
            var drawedCard = playerDeck.DrawTerrainCard();

            if (drawedCard is not null)
            {
                GameManager.instance.EndTurn();
            }
        }
    }
}
