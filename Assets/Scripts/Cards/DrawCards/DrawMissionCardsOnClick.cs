using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawMissionCardsOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var canMakeAction = !GameManager.instance.actionMade;
        if (PointerEventData.InputButton.Left == eventData.button && canMakeAction)
        {
            var missionCardsAvailable = GameManager.instance.missionCardsAvailables;
            var randomIndex = Random.Range(0, missionCardsAvailable.Count);
            var missionCardSelected = missionCardsAvailable[randomIndex];

            GameManager.instance.ActualPlayer.MissionCardsInPlayerHand.Add(missionCardSelected);
            GameManager.instance.missionCardsAvailables.Remove(missionCardSelected);
            GameManager.instance.actionMade = true;
        }
    }
}
