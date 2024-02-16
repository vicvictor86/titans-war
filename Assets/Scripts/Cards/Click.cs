using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour, IPointerClickHandler
{
    private PlayerDeck playerDeck;

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        playerDeck = player.GetComponent<PlayerDeck>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerDeck != null)
        {
            playerDeck.DiscartWarriorCard(this.gameObject.GetComponent<DisplayWarriorCard>().Card);
        }
        Destroy(this.gameObject);
    }
}
