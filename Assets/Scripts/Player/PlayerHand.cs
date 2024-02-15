using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<WarriorCard> WarriorsCards = new();
    private PlayerDeck deck;

    private void Start()
    {
        deck = GetComponent<PlayerDeck>();
        WarriorsCards = deck.DrawInitialsWarriorsCard();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            var drawedCard = deck.DrawWarriorCard();

            if (drawedCard != null)
            {
                WarriorsCards.Add(deck.DrawWarriorCard());
            }

        }
    }
}
