using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Players")]
    public PlayerDeck actualPlayer;
    public PlayerDeck NextPlayer => playerList[(actualPlayerIndex + 1) % playerList.Count];
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
        actualPlayerIndex = 0;
        actualPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeck>();
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
        actualPlayer = playerList[actualPlayerIndex];

        UIManager.instance.OpenMissionCardsToChoosePanel(actualPlayer.MissionCardsInPlayerHand);
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
        actualPlayer = playerList[actualPlayerIndex];
        actionMade = false;
        actualPlayer.Round();
    }

    private void PlayerEndRound()
    {
        actualPlayer = playerList[actualPlayerIndex];
        actualPlayer.EndRound();
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
        actualPlayer.StartAttackDefenseRound();
    }

    public void SetAttackDefenseCard(WarriorCard card)
    {
        if (attack)
        {
            Debug.Log("Setou a carta de ataque");
            attackingCard = card;
            attack = false;
            actualPlayer.EndAttackDefenseRound();
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
            actualPlayer.AddTerritory(contestedTerritory);
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
        actualPlayer.DiscartWarriorCard(attackingCard);
        playerList[(actualPlayerIndex + 1) % playerList.Count].DiscartWarriorCard(defendindCard);
        actualPlayer.DiscartTerrainCardByType(contestedTerritory.Type);
        attackingCard = null;
        defendindCard = null;
        contestedTerritory = null;
    }

    public void InstantiateCityInfo(City city, TerrainType terrainType, Territory territory)
    {
        Destroy(GameObject.FindWithTag("CityInfo"));
        Destroy(GameObject.FindWithTag("AttackButton"));
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        var infoInstance = Instantiate(city.cityInfoPrefab, cityInfoPanelPosition.position, Quaternion.identity, canvas.transform);
        infoInstance.GetComponent<DisplayCityInfo>().City = city;
        if (actualPlayer.ListTerrainTypesDisponibleToAttack().Contains(terrainType) && !actionMade)
        {
            var attackButton = Instantiate(city.attackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);
            attackButton.GetComponent<AttackButton>().territory = territory;
        };
    }
}
