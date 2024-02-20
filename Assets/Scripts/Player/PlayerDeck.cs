using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [Header("Warrior Cards")]
    private List<WarriorCard> availableInDeckWarriorCards = new();
    private List<WarriorCard> warriorCardsinPlayerHands = new();
    private List<WarriorCard> discartedWarriorCards = new();
    public int WarriorsInitialQuantity = 5;

    [Header("Terrain Cards")]
    private List<TerrainCard> availableInDeckTerrainCards = new();
    private List<TerrainCard> terrainCardsinPlayerHands = new();
    private List<TerrainCard> discartedTerrainCards = new();
    public int TerrainsInitialQuantity = 1;

    [SerializeField] private string playerSide = "Spartha";

    [Header("Prefabs")]
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private Transform playerHandPanelTransform;

    private void Start()
    {
        availableInDeckWarriorCards = playerSide == "Spartha" ? CardDatabase.SparthaCardList : CardDatabase.PersaCardList;
        availableInDeckTerrainCards = CardDatabase.TerrainCardsList;
        DrawInitialsWarriorsCard();
        DrawInitialsTerrainsCard();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            DrawWarriorCard();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            DrawTerrainCard();
        }
    }

    public List<WarriorCard> DrawInitialsWarriorsCard()
    {
        for (int i = WarriorsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, availableInDeckWarriorCards.Count);

            InstantiateNewWarriorCard(availableInDeckWarriorCards[randomCard]);

            warriorCardsinPlayerHands.Add(availableInDeckWarriorCards[randomCard]);
            availableInDeckWarriorCards.Remove(availableInDeckWarriorCards[randomCard]);
        }

        Debug.Log(availableInDeckWarriorCards.Count);
        return warriorCardsinPlayerHands;
    }

    public List<WarriorCard> DrawInitialsTerrainsCard()
    {
        for (int i = TerrainsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, availableInDeckTerrainCards.Count);

            InstantiateNewTerrainCard(availableInDeckTerrainCards[randomCard]);

            terrainCardsinPlayerHands.Add(availableInDeckTerrainCards[randomCard]);
            availableInDeckTerrainCards.Remove(availableInDeckTerrainCards[randomCard]);
        }

        Debug.Log(availableInDeckTerrainCards.Count);
        return warriorCardsinPlayerHands;
    }

    private void InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var cardInstance = Instantiate(warriorPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerHandPanelTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
        cardInstance.transform.SetAsFirstSibling();
    }

    private void InstantiateNewTerrainCard(TerrainCard terrainCard)
    {
        var cardInstance = Instantiate(terrainPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerHandPanelTransform);
        cardInstance.GetComponent<DisplayTerrainCard>().Card = terrainCard;
        cardInstance.transform.SetAsFirstSibling();
    }

    public WarriorCard DrawWarriorCard()
    {
        if (availableInDeckWarriorCards.Count <= 0)
        {
            if (discartedWarriorCards.Count > 0)
            {
                ResetWarriorDeck();
            } else
            {
                Debug.Log("No more cards available in deck");
                return null;
            }
        }

        WarriorCard cardDrawed = availableInDeckWarriorCards[0];

        warriorCardsinPlayerHands.Add(cardDrawed);
        availableInDeckWarriorCards.RemoveAt(0);

        InstantiateNewWarriorCard(cardDrawed);

        return cardDrawed;
    }

    public TerrainCard DrawTerrainCard()
    {
        if (availableInDeckTerrainCards.Count <= 0)
        {
            if (discartedTerrainCards.Count > 0)
            {
                ResetTerrainDeck();
            }
            else
            {
                Debug.Log("No more cards available in deck");
                return null;
            }
        }

        TerrainCard cardDrawed = availableInDeckTerrainCards[0];

        terrainCardsinPlayerHands.Add(cardDrawed);
        availableInDeckTerrainCards.RemoveAt(0);

        InstantiateNewTerrainCard(cardDrawed);

        return cardDrawed;
    }

    public void DiscartCard(Card card)
    {
        switch (card)
        {
            case WarriorCard wc:
                DiscartWarriorCard(wc);
                break;
            case TerrainCard tc:
                DiscartTerrainCard(tc);
                break;
            default: 
                throw new System.Exception($"Tipo de card não aceito na função {nameof(DiscartCard)}");
        }
    }

    public void DiscartWarriorCard(WarriorCard warriorCardDiscarted)
    {
        warriorCardsinPlayerHands.Remove(warriorCardDiscarted);
        discartedWarriorCards.Add(warriorCardDiscarted);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted)
    {
        terrainCardsinPlayerHands.Remove(terrainCardDiscarted);
        discartedTerrainCards.Add(terrainCardDiscarted);
    }

    private void ResetWarriorDeck()
    {
        foreach(var item in discartedWarriorCards)
        {
            availableInDeckWarriorCards.Add(item);
        }

        discartedWarriorCards.Clear();
    }

    private void ResetTerrainDeck()
    { 
        foreach (var item in discartedTerrainCards)
        {
            availableInDeckTerrainCards.Add(item);
        }

        discartedTerrainCards.Clear();
    }
}
