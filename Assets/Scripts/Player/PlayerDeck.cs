using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private List<WarriorCard> availableInDeckWarriorCards = new();
    private List<WarriorCard> inPlayerHandsCards = new();
    private List<WarriorCard> discartedWarriorCards = new();
    
    private Transform firstCardDrawedTransform;
    public int InitialQuantity = 5;

    [SerializeField] private string playerSide = "Spartha";
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private Transform canvasTransform;

    private void Start()
    {
        availableInDeckWarriorCards = playerSide == "Spartha" ? CardDatabase.SparthaCardList : CardDatabase.PersaCardList;
        DrawInitialsWarriorsCard();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            DrawWarriorCard();
        }
    }

    public List<WarriorCard> DrawInitialsWarriorsCard()
    {
        for (int i = InitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, availableInDeckWarriorCards.Count);

            var cardInstance = Instantiate(warriorPrefab, new Vector3(i, 0, 0), Quaternion.identity, canvasTransform);
            cardInstance.GetComponent<DisplayWarriorCard>().Card = availableInDeckWarriorCards[randomCard];

            inPlayerHandsCards.Add(availableInDeckWarriorCards[randomCard]);
            availableInDeckWarriorCards.Remove(availableInDeckWarriorCards[randomCard]);

            if (i == 0)
            {
                firstCardDrawedTransform = cardInstance.transform;
            }
        }

        Debug.Log(availableInDeckWarriorCards.Count);
        return inPlayerHandsCards;
    }

    private void InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var firstCardDrawedPositionX = firstCardDrawedTransform != null ? firstCardDrawedTransform.position.x : 0;
        var cardInstance = Instantiate(warriorPrefab, new Vector3(firstCardDrawedPositionX - 2, 0, 0), Quaternion.identity, canvasTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
    }

    private WarriorCard DrawWarriorCard()
    {
        if (availableInDeckWarriorCards.Count <= 0)
        {
            if (discartedWarriorCards.Count > 0)
            {
                ResetDeck();
            } else
            {
                Debug.Log("No more cards available in deck");
                return null;
            }
        }

        WarriorCard cardDrawed = availableInDeckWarriorCards[0];

        inPlayerHandsCards.Add(cardDrawed);
        availableInDeckWarriorCards.RemoveAt(0);

        InstantiateNewWarriorCard(cardDrawed);

        return cardDrawed;
    }

    public void DiscartWarriorCard(WarriorCard warriorCardDiscarted)
    {
        inPlayerHandsCards.Remove(warriorCardDiscarted);
        discartedWarriorCards.Add(warriorCardDiscarted);
    }

    private void ResetDeck()
    {
        foreach(var item in discartedWarriorCards)
        {
            availableInDeckWarriorCards.Add(item);
        }

        discartedWarriorCards.Clear();
    }

}
