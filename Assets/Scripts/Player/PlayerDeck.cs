using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [Header("Warrior Cards")]
    private List<WarriorCard> availableInDeckWarriorCards = new();
    private List<WarriorCard> warriorCardsinPlayerHands = new();
    private List<WarriorCard> discartedWarriorCards = new();
    private readonly int warriorsInitialQuantity = 1;

    [Header("Terrain Cards")]
    private List<TerrainCard> terrainCardsinPlayerHands = new();
    private List<TerrainCard> discartedTerrainCards = new();
    private readonly int terrainsInitialQuantity = 1;

    private int riverCardsQuantity = 0;
    private int mountainCardsQuantity = 0;
    private int plainsCardsQuantity = 0;
    private int desertCardsQuantity = 0;

    [SerializeField] private string playerSide = "Spartha";

    [Header("Mission Cards")]
    public List<MissionCard> missionCardsinPlayerHands = new();
    private int missionCardsInitialQuantity = 3;
    [SerializeField] public Transform missionCardPlace;
    [SerializeField] private GameObject missionPrefab;

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
            int randomCard = UnityEngine.Random.Range(0, availableInDeckWarriorCards.Count);

            InstantiateNewWarriorCard(availableInDeckWarriorCards[randomCard]);

            warriorCardsinPlayerHands.Add(availableInDeckWarriorCards[randomCard]);
            availableInDeckWarriorCards.Remove(availableInDeckWarriorCards[randomCard]);
        }

        return warriorCardsinPlayerHands;
    }

    public List<TerrainCard> DrawInitialsTerrainsCard(Dictionary<TerrainType, TerrainCardDeck> terrainCardsAvailableInDeck)
    {
        for (int i = terrainsInitialQuantity; i > 0; i--)
        {
            TerrainType terrainSelected = TerrainCardDeck.SelectRandomTerrainType();

            AddTerrainCardToHand(terrainSelected);
        }

        UIManager.instance.UpdateTerrainCards(riverCardsQuantity, mountainCardsQuantity, plainsCardsQuantity, desertCardsQuantity);

        return terrainCardsinPlayerHands;
    }

    public List<MissionCard> DrawInitialsMissionsCard(List<MissionCard> missionCardsAvailable)
    {
        for (int i = missionCardsInitialQuantity; i > 0; i--)
        {
            int randomCard = UnityEngine.Random.Range(0, missionCardsAvailable.Count);

            var cardSelected = missionCardsAvailable[randomCard];
            InstantiateNewMissionCard(cardSelected);

            missionCardsinPlayerHands.Add(cardSelected);
            missionCardsAvailable.Remove(missionCardsAvailable[randomCard]);
        }

        return missionCardsinPlayerHands;
    }

    private GameObject InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var cardInstance = Instantiate(warriorPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerWarriorHandPanelTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
        cardInstance.transform.SetAsFirstSibling();

        return cardInstance;
    }

    private GameObject InstantiateNewMissionCard(MissionCard missionCard)
    {
        var cardInstance = Instantiate(missionPrefab, missionCardPlace.position, Quaternion.identity, missionCardPlace.transform);
        cardInstance.GetComponent<DisplayMissionCard>().Card = missionCard;
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
        if (GameManager.instance.terrainCardsAvailable.Count <= 0)
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

        TerrainType terrainSelected = TerrainCardDeck.SelectRandomTerrainType();
        AddTerrainCardToHand(terrainSelected);

        Debug.Log(terrainSelected);

        var cardDrawed = GameManager.instance.terrainCardsAvailable[terrainSelected].terrainCard;

        Debug.Log(cardDrawed);

        Debug.Log(GameManager.instance.terrainCardsAvailable[terrainSelected].quantityCard);

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

    public void DiscartMissionCard(MissionCard missionCardToDiscart)
    {
        var missionHand = missionCardPlace.GetComponentsInChildren<DisplayMissionCard>();
        var missionCard = missionHand.FirstOrDefault(displayCard => displayCard.Card == missionCardToDiscart);
        Destroy(missionCard.gameObject);
        missionCardsinPlayerHands.Remove(missionCardToDiscart);
        GameManager.instance.missionCardsAvailables.Add(missionCardToDiscart);
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
            //availableInDeckTerrainCards.Add(item);
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

    private void AddTerrainCardToHand(TerrainType terrainType)
    {
        GameManager.instance.terrainCardsAvailable[terrainType].quantityCard--;
        terrainCardsinPlayerHands.Add(GameManager.instance.terrainCardsAvailable[terrainType].terrainCard);

        switch (terrainType)
        {
            case TerrainType.RIVER:
                Debug.Log("Novo river");
                riverCardsQuantity++;
                break;
            case TerrainType.MOUNTAINS:
                Debug.Log("Novo mountain");
                mountainCardsQuantity++;
                break;
            case TerrainType.PLAINS:
                Debug.Log("Novo plains");
                plainsCardsQuantity++;
                break;
            case TerrainType.DESERT:
                Debug.Log("Novo desert");
                desertCardsQuantity++;
                break;
            default:
                break;
        }

        UIManager.instance.UpdateTerrainCards(riverCardsQuantity, mountainCardsQuantity, plainsCardsQuantity, desertCardsQuantity);

    }
}
