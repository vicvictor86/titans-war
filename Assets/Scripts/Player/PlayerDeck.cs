using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [Header("Warrior Cards")]
    public List<WarriorCard> DiscartedWarriorCards = new();
    public List<WarriorCard> WarriorCardsAvailableInDeck { get; set; } = new();
    public List<WarriorCard> WarriorCardsInPlayerHand { get; private set; } = new();
    public int WarriorsInitialQuantity { get; } = 1;

    [Header("Terrain Cards")]
    public int RiverCardsQuantity = 0;
    public int MountainCardsQuantity = 0;
    public int PlainsCardsQuantity = 0;
    public int DesertCardsQuantity = 0;
    public List<TerrainCard> TerrainCardsInPlayerHand { get; set; } = new();
    public int terrainsInitialQuantity { get; } = 1;


    [SerializeField] public string PlayerSide { get; } = "Spartha";

    [Header("Mission Cards")]
    public List<MissionCard> MissionCardsInPlayerHand = new();
    public int MissionCardsInitialQuantity { get; private set; } = 3;
    [SerializeField] public Transform missionCardPlace;
    [SerializeField] private GameObject missionPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private Transform playerWarriorHandPanelTransform;
    [SerializeField] private Transform playerTerrainHandPanelTransform;

    private List<Territory> territoriesWithPlayer = new();

    [Header("Related Scripts")]
    [SerializeField] private DiscartCard discartCards;
    [SerializeField] private DrawCard drawCard;
    [SerializeField] private InstantiateCard instantiateCard;

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
        drawCard.DrawInitialsWarriorsCard(this);

        return WarriorCardsInPlayerHand;
    }

    public int GetPlayerPoints()
    {
        int totalPoints = 0;
        foreach(var territory in territoriesWithPlayer)
        {
            totalPoints += territory.Point;
        }

        return totalPoints;
    }

    public List<TerrainCard> DrawInitialsTerrainsCard()
    {
        drawCard.DrawInitialsTerrainsCard(this);

        UIManager.instance.UpdateTerrainCards(RiverCardsQuantity, MountainCardsQuantity, PlainsCardsQuantity, DesertCardsQuantity);

        return TerrainCardsInPlayerHand;
    }

    public List<MissionCard> DrawInitialsMissionsCard(List<MissionCard> missionCardsAvailable)
    {
        drawCard.DrawInitialsMissionsCard(missionCardsAvailable, this);

        return MissionCardsInPlayerHand;
    }

    public GameObject InstantiateNewWarriorCard(WarriorCard warriorCard)
    {
        var cardInstance = instantiateCard.InstantiateNewWarriorCard(warriorCard, warriorPrefab, playerWarriorHandPanelTransform);

        return cardInstance;
    }

    public GameObject InstantiateNewMissionCard(MissionCard missionCard)
    {
        var cardInstance = instantiateCard.InstantiateNewMissionCard(missionCard, missionPrefab, missionCardPlace);

        return cardInstance;
    }

    public WarriorCard DrawWarriorCard()
    {
        var cardDrawed = drawCard.DrawWarriorCard(this);

        return cardDrawed;
    }

    public TerrainCard DrawTerrainCard()
    {
        var cardDrawed = drawCard.DrawTerrainCard(this);

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
        discartCards.DiscartWarriorCard(warriorCardDiscarted, playerWarriorHandPanelTransform, WarriorCardsInPlayerHand, DiscartedWarriorCards);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted)
    {
        discartCards.DiscartTerrainCard(terrainCardDiscarted, TerrainCardsInPlayerHand, this);
    }

    public void DiscartMissionCard(MissionCard missionCardToDiscart)
    {
        discartCards.DiscartMissionCard(missionCardToDiscart, missionCardPlace, MissionCardsInPlayerHand);
    }

    public void DiscartTerrainCardByType(TerrainType type)
    {
        discartCards.DiscartTerrainCardByType(type, playerTerrainHandPanelTransform, TerrainCardsInPlayerHand, this);
    }

    public void ResetWarriorDeck()
    {
        foreach(var item in DiscartedWarriorCards)
        {
            WarriorCardsAvailableInDeck.Add(item);
        }

        DiscartedWarriorCards.Clear();
    }

    public void Round()
    {
        
    }

    public void EndRound()
    {
        
    }

    public List<TerrainCard> ListTerrainCardsInHand()
    {
        return  TerrainCardsInPlayerHand;
    }

    public List<TerrainType> ListTerrainTypesDisponibleToAttack()
    {
        return ListTerrainCardsInHand().Select(card => card.Type).Distinct().ToList();
    }

    public List<WarriorCard> ListWarriorsCardsInHand()
    {
        return WarriorCardsInPlayerHand;
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
        UIManager.instance.UpdatePlayerTotalPoints(GetPlayerPoints());
    }

    public List<Territory> GetTerritoriesWithPlayer()
    {
        return territoriesWithPlayer;
    }
    
}
