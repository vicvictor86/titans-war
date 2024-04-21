using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawWarriorCardOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var playerDeck = GameManager.instance.myPlayer;
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        bool canDrawCard = GameManager.instance.CanDrawCard(playerDeck);
        if (clickWithLeftButton && canDrawCard && playerDeck != null)
        {
            var drawnCard = playerDeck.DrawWarriorCard();
            GameManager.instance.photonView.RPC(nameof(GameManager.instance.SendOpponentWarriorCardsQuantity), Photon.Pun.RpcTarget.Others, playerDeck.WarriorCardsInPlayerHand.Count);

            if (drawnCard != null)
            {
                GameManager.instance.EndTurn();
            }
        }
    }
}
