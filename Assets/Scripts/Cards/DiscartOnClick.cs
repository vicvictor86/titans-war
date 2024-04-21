using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscartOnClick : MonoBehaviour, IPointerClickHandler
{
    private PlayerDeck playerDeck;

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        playerDeck = player.GetComponent<PlayerDeck>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        if (clickWithLeftButton && playerDeck != null)
        {
            var displayWarriorCard = gameObject.GetComponent<DisplayWarriorCard>();
            var displayTerrainCard = gameObject.GetComponent<DisplayTerrainCard>();
            if (displayWarriorCard != null)
            {
                playerDeck.DiscartCard(displayWarriorCard.Card);
            }
            else if (displayTerrainCard != null)
            {
                playerDeck.DiscartCard(displayTerrainCard.Card);
            }

            Destroy(gameObject);
        }
    }
}
