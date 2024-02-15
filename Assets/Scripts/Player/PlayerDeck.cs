using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private List<WarriorCard> notDrawedWarriorCards = new();
    private List<WarriorCard> drawedWarriorCards = new();
    private Transform firstCardDrawedTransform;
    public int InitialQuantity = 5;
    [SerializeField] private string playerSide = "Spartha";
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private Transform canvasTransform;

    private void Start()
    {
        notDrawedWarriorCards = playerSide == "Spartha" ? CardDatabase.SparthaCardList : CardDatabase.PersaCardList;
    }

    public List<WarriorCard> DrawInitialsWarriorsCard()
    {
        for (int i = 0; i < InitialQuantity; i++)
        {
            int randomCard = Random.Range(0, notDrawedWarriorCards.Count);

            var cardInstance = Instantiate(warriorPrefab, new Vector3(i, 0, 0), Quaternion.identity, canvasTransform);
            cardInstance.GetComponent<DisplayWarriorCard>().Card = notDrawedWarriorCards[randomCard];

            drawedWarriorCards.Add(notDrawedWarriorCards[randomCard]);
            notDrawedWarriorCards.Remove(notDrawedWarriorCards[randomCard]);

            if (i == 0)
            {
                firstCardDrawedTransform = cardInstance.transform;
            }
        }

        Debug.Log(notDrawedWarriorCards.Count);
        return drawedWarriorCards;
    }

    private void InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var cardInstance = Instantiate(warriorPrefab, new Vector3(firstCardDrawedTransform.position.x - 2, 0, 0), Quaternion.identity, canvasTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
    }

    public WarriorCard DrawWarriorCard()
    {
        if (notDrawedWarriorCards.Count <= 0)
        {
            Debug.Log("No more cards available in deck");
            return null;
        }

        WarriorCard cardDrawed = notDrawedWarriorCards[0];

        drawedWarriorCards.Add(cardDrawed);
        notDrawedWarriorCards.RemoveAt(0);

        InstantiateNewWarriorCard(cardDrawed);

        return cardDrawed;
    }

}
