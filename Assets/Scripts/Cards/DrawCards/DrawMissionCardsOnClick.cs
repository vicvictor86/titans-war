using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawMissionCardsOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var playerDeck = GameManager.instance.ActualPlayer;
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        bool canDrawCard = GameManager.instance.CanDrawCard(playerDeck);
        if (clickWithLeftButton && canDrawCard && playerDeck != null && playerDeck.WarriorCardsInPlayerHand.Any())
        {
            var missionCardsAvailable = GameManager.instance.missionCardsAvailables;

            if(missionCardsAvailable.Count <= 0)
            {
                return;
            }

            var randomIndex = Random.Range(0, missionCardsAvailable.Count);
            var missionCardSelected = missionCardsAvailable[randomIndex];

            GameManager.instance.ActualPlayer.MissionCardsInPlayerHand.Add(missionCardSelected);
            GameManager.instance.missionCardsAvailables.Remove(missionCardSelected);

            //if (GameManager.instance.actualPlayerIndex == GameManager.instance.playerList.IndexOf(GameManager.instance.playerList.FirstOrDefault(player => player.PlayerSide == "Spartha"))) 
            //{
            //    UIManager.instance.UpdateMissionCardsScroller(missionCardSelected);
            //}

            GameManager.instance.EndTurn();
        }
    }
}
