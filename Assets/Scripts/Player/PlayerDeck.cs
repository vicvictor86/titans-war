using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeck : MonoBehaviour
{
    [Header("Player Status")]
    [SerializeField] public string PlayerSide;

    [Header("Warrior Cards")]
    public List<WarriorCard> DiscartedWarriorCards = new();
    public List<WarriorCard> WarriorCardsAvailableInDeck { get; set; } = new();
    public List<WarriorCard> WarriorCardsInPlayerHand { get; private set; } = new();
    public int WarriorsInitialQuantity { get; } = 12;

    [Header("Terrain Cards")]
    public int RiverCardsQuantity  = 0;
    public int MountainCardsQuantity = 0;
    public int PlainsCardsQuantity = 0;
    public int DesertCardsQuantity = 0;
    public List<TerrainCard> TerrainCardsInPlayerHand { get; set; } = new();
    public int terrainsInitialQuantity { get; } = 30;
    

    [Header("Mission Cards")]
    public List<MissionCard> MissionCardsInPlayerHand = new();
    public int MissionCardsInitialQuantity { get; private set; } = 3;
    [SerializeField] public Transform missionCardPlace;
    [SerializeField] private GameObject missionPrefab;

    [Header("Power Cards")]
    public List<PowerCard> PowerCardsInPlayerHand = new();
    public Dictionary<int, int> PowerCards = new() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

    [Header("Prefabs")]
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private Transform playerWarriorHandPanelTransform;
    [SerializeField] private Transform playerTerrainHandPanelTransform;

    private List<Territory> territoriesWithPlayer = new();

    [Header("UI")]
    [SerializeField] public Image turnIcon;

    [Header("Related Scripts")]
    [SerializeField] private DiscartCard discartCards;
    [SerializeField] private DrawCard drawCard;
    [SerializeField] private InstantiateCard instantiateCard;

    private void Start()
    {
        turnIcon.sprite = null;
        turnIcon.color = new Color(0, 0, 0, 0);
    }

    [Header("Text")]
    public TextMeshProUGUI FinalPoints;

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
        Debug.Log(drawCard);
        drawCard.DrawInitialsWarriorsCard(this);

        return WarriorCardsInPlayerHand;
    }

    public List<TerrainCard> DrawInitialsTerrainsCard()
    {
        drawCard.DrawInitialsTerrainsCard(this);

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
                throw new System.Exception($"Tipo de card n�o aceito na fun��o {nameof(DiscartCard)}");
        }
    }

    public void DiscartWarriorCard(WarriorCard warriorCardDiscarted)
    {
        discartCards.DiscartWarriorCard(warriorCardDiscarted, playerWarriorHandPanelTransform, WarriorCardsInPlayerHand, DiscartedWarriorCards);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted)
    {
        discartCards.DiscartTerrainCard(terrainCardDiscarted, playerTerrainHandPanelTransform,  TerrainCardsInPlayerHand);
    }

    public void DiscartMissionCard(MissionCard missionCardToDiscart)
    {
        discartCards.DiscartMissionCard(missionCardToDiscart, missionCardPlace, MissionCardsInPlayerHand);
    }

    public void DiscartTerrainCardByType(TerrainType type)
    {
        discartCards.DiscartTerrainCardByType(type, playerTerrainHandPanelTransform, TerrainCardsInPlayerHand);
        RemoveTerrainCard(type);
    }

    public void RemoveTerrainCard(TerrainType type)
    {
        switch (type){
            case TerrainType.RIVER:
                RiverCardsQuantity--;
                break;
            case TerrainType.PLAINS:
                PlainsCardsQuantity--; 
                break;
            case TerrainType.DESERT:
                DesertCardsQuantity--;
                break;
            case TerrainType.MOUNTAINS:
                MountainCardsQuantity--;
                break;
        }
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
        territory.City.ValidAndSetOwnership(this);
    }

    public List<Territory> GetTerritoriesWithPlayer()
    {
        return territoriesWithPlayer;
    }

    public int GetPoints()
    {
        var cities = GameObject.FindGameObjectsWithTag("City").Select(gameObject => gameObject.GetComponent<City>());
        return cities.Sum(city => city.GetPointsForPlayer(this));
    }

    public void AddNewPowerCard(PowerCard powerCard)
    {
        PowerCardsInPlayerHand.Add(powerCard);
        PowerCards[powerCard.ExtraPoints]++;
    }

    public void RemovePowerCard(int extraPoints)
    {
        if (extraPoints == 0) return;

        PowerCardsInPlayerHand.Remove(PowerCardsInPlayerHand.First(powerCard => powerCard.ExtraPoints == extraPoints));
        PowerCards[extraPoints]--;
    }
}
