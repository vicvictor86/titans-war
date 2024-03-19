using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    
    public PlayerDeck ActualPlayer => playerList[actualPlayerIndex];
    public PlayerDeck NextPlayer => playerList[(actualPlayerIndex + 1) % playerList.Count];
    [Header("Players")]
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;

    [Header("Attack/Defense")]
    public bool actionMade = false;
    public WarriorCard attackingCard;
    public WarriorCard defendindCard;
    public Territory contestedTerritory = null;
    public bool attack = false;

    [Header("Positions")]
    public Transform cityInfoPanelPosition;
    public Transform territoryInfoPanelPosition;
    public Transform attackButtonPosition;

    [Header("States")]
    public bool needSelectMissionCard;
    public int playerAction = 0;

    [Header("Cards Available")]
    public List<MissionCard> missionCardsAvailables;
    public Dictionary<TerrainType, TerrainCardDeck> terrainCardsAvailable = new();
    public List<DisplayMissionCard> missionCardsToChoose = new();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player").Select(PlayerDeck => PlayerDeck.GetComponent<PlayerDeck>()).ToList();

        missionCardsAvailables = new (CardDatabase.MissionCardList);
        
        foreach(var card in CardDatabase.TerrainCardsList)
        {
            terrainCardsAvailable.Add(card.Type, new TerrainCardDeck(card, 20));
        }

        FirstRound();
    }

    void FirstRound()
    {
        foreach (var player in playerList)
        {
            player.DrawInitialsWarriorsCard();
            player.DrawInitialsTerrainsCard();
            player.DrawInitialsMissionsCard(missionCardsAvailables);
        }

        actualPlayerIndex = Random.Range(0, playerList.Count);

        UIManager.instance.OpenMissionCardsToChoosePanel(ActualPlayer.MissionCardsInPlayerHand);
        ChangeDisplayMissionToClickable(missionCardsToChoose);

        needSelectMissionCard = true;
    }

    private void ChangeDisplayMissionToClickable(List<DisplayMissionCard> displayMissionCards)
    {
        foreach (var displayMissionCardActual in displayMissionCards)
        {
            displayMissionCardActual.isClickable = true;
        }
    }

    public void EndMissionSelection()
    {
        Debug.Log($"Usuário {playerList[(actualPlayerIndex + playerAction) % playerList.Count]}");
        int missionCardsSelected = 0;
        foreach (var displayMissionCardActual in missionCardsToChoose)
        {
            if (displayMissionCardActual.IsSelected)
            {
                missionCardsSelected++;
            }
        }

        if (missionCardsSelected == 0)
        {
            Debug.LogError("You have to select at least one mission card.");
            return;
        }

        foreach (var displayMissionCardActual in missionCardsToChoose)
        {
            if (!displayMissionCardActual.IsSelected)
            {
                playerList[(actualPlayerIndex + playerAction) % playerList.Count].DiscartMissionCard(displayMissionCardActual.Card);
            }

            displayMissionCardActual.isClickable = false;
        }

        playerAction++;

        if (playerAction == playerList.Count)
        {
            EndFirstRound();
        } 
        else
        {
            ChangeDisplayMissionToClickable(NextPlayer.missionCardPlace.GetComponentsInChildren<DisplayMissionCard>().ToList());

            UIManager.instance.OpenMissionCardsToChoosePanel(NextPlayer.MissionCardsInPlayerHand);
            ChangeDisplayMissionToClickable(missionCardsToChoose);
        }
    }

    private void EndFirstRound()
    {
        UIManager.instance.CloseMissionCardsToChoosePanel();
        PlayerRound();
    }

    private void PlayerRound()
    {
        actionMade = false;
        ActualPlayer.Round();
    }

    private void PlayerEndRound()
    {
        ActualPlayer.EndRound();
    }

    public void EndTurn()
    {
        PlayerEndRound();

        actualPlayerIndex = (actualPlayerIndex + 1) % playerList.Count;

        PlayerRound();
    }

    public bool CanDrawCard(PlayerDeck playerdDeck)
    {
        var canDrawCard = !actionMade && playerdDeck == playerList[actualPlayerIndex];
        return canDrawCard;
    }

    public void AttackRound(Territory territory)
    {
        actionMade = true;
        contestedTerritory = territory;
        attack = true;
        Debug.Log("Pode escolher a carta");
        ActualPlayer.StartAttackDefenseRound();
    }

    public void SetAttackDefenseCard(WarriorCard card)
    {
        if (attack)
        {
            Debug.Log("Setou a carta de ataque");
            attackingCard = card;
            attack = false;
            ActualPlayer.EndAttackDefenseRound();
            Debug.Log("Defesa pode escolher a carta");
            playerList[(actualPlayerIndex + 1) % playerList.Count].StartAttackDefenseRound();
        }
        else
        {
            Debug.Log("Setou a carta de defesa");
            defendindCard = card;
            playerList[(actualPlayerIndex + 1) % playerList.Count].EndAttackDefenseRound();
            CalculateWinner();
        }
    }

    public void CalculateWinner()
    {
        var attackValue = attackingCard.GetPowerValue(contestedTerritory.Type);
        var defenseValue = defendindCard.GetPowerValue(contestedTerritory.Type);
        if (attackValue > defenseValue)
        {
            Debug.Log("Ataque venceu");
            ActualPlayer.AddTerritory(contestedTerritory);
        }
        else if (defenseValue > attackValue)
        {
            Debug.Log("Defesa Venceu");
            if (playerList[(actualPlayerIndex + 1) % playerList.Count].GetTerritoriesWithPlayer().Contains(contestedTerritory))
            {
                //Adicionar carta de poder
            }
            else
            {
                playerList[(actualPlayerIndex + 1) % playerList.Count].AddTerritory(contestedTerritory);
            }
        }
        else {
            Debug.Log("Empate");
        }
        ActualPlayer.DiscartWarriorCard(attackingCard);
        playerList[(actualPlayerIndex + 1) % playerList.Count].DiscartWarriorCard(defendindCard);
        ActualPlayer.DiscartTerrainCardByType(contestedTerritory.Type);
        Destroy(GameObject.FindWithTag("AttackButton"));
        attackingCard = null;
        defendindCard = null;
        contestedTerritory = null;
    }

    public void InstantiateCityAndTerritoryInfo(City city, TerrainType terrainType, Territory territory)
    {
        Destroy(GameObject.FindWithTag("CityInfo"));
        Destroy(GameObject.FindWithTag("TerritoryInfo"));
        Destroy(GameObject.FindWithTag("AttackButton"));
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        var cityInfoInstance = Instantiate(city.cityInfoPrefab, cityInfoPanelPosition.position, Quaternion.identity, canvas.transform);
        cityInfoInstance.GetComponent<DisplayCityInfo>().City = city;
        var territoryInfoInstace = Instantiate(territory.territoryInfoPrefab, territoryInfoPanelPosition.position, Quaternion.identity, canvas.transform);
        territoryInfoInstace.GetComponent<DisplayTerritoryInfo>().Territory = territory;
        if (ActualPlayer.ListTerrainTypesDisponibleToAttack().Contains(terrainType) &&
            !actionMade &&
            ActualPlayer.WarriorCardsInPlayerHand.Any() &&
            territory.Owner != ActualPlayer)
        {
            var attackButton = Instantiate(city.attackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);
            attackButton.GetComponent<AttackButton>().territory = territory;
        };
    }

}
