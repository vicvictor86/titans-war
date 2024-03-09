using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [Header("Warrior Cards")]
    private List<WarriorCard> availableInDeckWarriorCards = new();
    private List<WarriorCard> warriorCardsinPlayerHands = new();
    private List<WarriorCard> discartedWarriorCards = new();
    private readonly int warriorsInitialQuantity = 1;

    [Header("Terrain Cards")]
    private List<TerrainCard> availableInDeckTerrainCards = new();
    private List<TerrainCard> terrainCardsinPlayerHands = new();
    private List<TerrainCard> discartedTerrainCards = new();
    private readonly int terrainsInitialQuantity = 1;

    [SerializeField] private string playerSide = "Spartha";

    [Header("Prefabs")]
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private Transform playerWarriorHandPanelTransform;
    [SerializeField] private Transform playerTerrainHandPanelTransform;

    private List<Territory> territoriesWithPlayer = new();

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
        availableInDeckWarriorCards = playerSide == "Spartha" ? CardDatabase.SparthaCardList : CardDatabase.PersaCardList;

        for (int i = warriorsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, availableInDeckWarriorCards.Count);

            InstantiateNewWarriorCard(availableInDeckWarriorCards[randomCard]);

            warriorCardsinPlayerHands.Add(availableInDeckWarriorCards[randomCard]);
            availableInDeckWarriorCards.Remove(availableInDeckWarriorCards[randomCard]);
        }

        return warriorCardsinPlayerHands;
    }

    public List<WarriorCard> DrawInitialsTerrainsCard()
    {
        availableInDeckTerrainCards = CardDatabase.TerrainCardsList;

        for (int i = terrainsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, availableInDeckTerrainCards.Count);

            InstantiateNewTerrainCard(availableInDeckTerrainCards[randomCard]);

            terrainCardsinPlayerHands.Add(availableInDeckTerrainCards[randomCard]);
            availableInDeckTerrainCards.Remove(availableInDeckTerrainCards[randomCard]);
        }

        return warriorCardsinPlayerHands;
    }

    private GameObject InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var cardInstance = Instantiate(warriorPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerWarriorHandPanelTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
        cardInstance.transform.SetAsFirstSibling();

        return cardInstance;
    }

    private GameObject InstantiateNewTerrainCard(TerrainCard terrainCard)
    {
        var cardInstance = Instantiate(terrainPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerTerrainHandPanelTransform);
        cardInstance.GetComponent<DisplayTerrainCard>().Card = terrainCard;
        cardInstance.transform.SetAsFirstSibling();

        return cardInstance;
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

        var warriorCardInstance = InstantiateNewWarriorCard(cardDrawed);
        warriorCardInstance.GetComponent<DragCards>().IsDraggable = true;

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

        var terrainCardInstance = InstantiateNewTerrainCard(cardDrawed);

        terrainCardInstance.GetComponent<DragCards>().IsDraggable = true;

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
                throw new System.Exception($"Tipo de card n�o aceito na fun��o {nameof(DiscartCard)}");
        }
    }

    public void DiscartWarriorCard(WarriorCard warriorCardDiscarted)
    {
        var warriorHand = playerWarriorHandPanelTransform.GetComponentsInChildren<DisplayWarriorCard>();
        var warriorCard = warriorHand.FirstOrDefault(displayCard => displayCard.Card == warriorCardDiscarted);
        Destroy(warriorCard.gameObject);
        warriorCardsinPlayerHands.Remove(warriorCardDiscarted);
        discartedWarriorCards.Add(warriorCardDiscarted);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted)
    {
        var terrainHand = playerTerrainHandPanelTransform.GetComponentsInChildren<DisplayTerrainCard>();
        var terrainCard = terrainHand.FirstOrDefault(displayCard => displayCard.Card == terrainCardDiscarted);
        Destroy(terrainCard.gameObject);
        terrainCardsinPlayerHands.Remove(terrainCardDiscarted);
        discartedTerrainCards.Add(terrainCardDiscarted);
    }

    public void DiscartTerrainCardByType(TerrainType type)
    {

        var terrainHand = playerTerrainHandPanelTransform.GetComponentsInChildren<DisplayTerrainCard>();
        var terrainCard = terrainHand.FirstOrDefault(displayCard => displayCard.Card.Type == type);
        DiscartTerrainCard(terrainCard.Card);
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

    public void Round()
    {
        
    }

    public void EndRound()
    {
        
    }

    public List<TerrainCard> ListTerrainCardsInHand()
    {
        return terrainCardsinPlayerHands;
    }

    public List<TerrainType> ListTerrainTypesDisponibleToAttack()
    {
        return ListTerrainCardsInHand().Select(card => card.Type).Distinct().ToList();
    }

    public List<WarriorCard> ListWarriorsCardsInHand()
    {
        return warriorCardsinPlayerHands;
    }

    public void StartAttackDefenseRound() {
        var playerCards = playerWarriorHandPanelTransform.GetComponentsInChildren<AttackDefense>();
        foreach (var card in playerCards)
        {
            card.isClickable = true;
        }
    }

    public void EndAttackDefenseRound()
    {
        var playerCards = playerWarriorHandPanelTransform.GetComponentsInChildren<AttackDefense>();
        foreach (var card in playerCards)
        {
            card.isClickable = false;
        }
    }

    public void AddTerritory(Territory territory)
    {
        territoriesWithPlayer.Add(territory);
        territory.SetOwner(this);
    }

    public List<Territory> GetTerritoriesWithPlayer()
    {
        return territoriesWithPlayer;
    }
}
