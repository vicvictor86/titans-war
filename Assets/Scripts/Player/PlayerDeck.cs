using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private List<Card> cards = new();
    public int initialQuantity = 5;

    private void Start()
    {
        for (int i = 0; i < initialQuantity; i++)
        {
            int randomCard = Random.Range(0, 2);
            cards.Add(CardDatabase.cardsList[randomCard]);
            Debug.Log(cards[i]);
        }
    }

    public Card DrawCard()
    {
        Card cardDrawed = cards[0];
        
        cards.RemoveAt(0);

        return cardDrawed;
    }

}
