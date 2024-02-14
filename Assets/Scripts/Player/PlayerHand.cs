using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> cards = new();
    private PlayerDeck deck;

    private void Start()
    {
        deck = GetComponent<PlayerDeck>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            cards.Add(deck.DrawCard());
        }
    }
}
