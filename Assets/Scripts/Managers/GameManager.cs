using Assets.Scripts.Cards.Missions;
using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private int extraPower = 0;
    [SerializeField] private TextMeshProUGUI endAttackTurnText;

    [SerializeField] private Button endTurnButton;

    [Header("Positions")]
    public Transform cityInfoPanelPosition;
    public Transform territoryInfoPanelPosition;
    public Transform attackButtonPosition;

    [Header("States")]
    public bool needSelectMissionCard;
    public int playerAction = 0;
    public bool candEndTurn = false;
    public bool isOnBattle = false;

    [Header("Cards Available")]
    public List<MissionCard> missionCardsAvailables;
    public Dictionary<TerrainType, TerrainCardDeck> terrainCardsAvailable = new();
    public List<DisplayMissionCard> missionCardsToChoose = new();
    public List<PowerCard> powerCardsAvailable = new();

    const string playerTurn = "playerTurn";
    const string defenseTurn = "defenseTurn";
    const string attackTurn = "attackTurn";
    const string looser = "looser";
    const string winner = "winner";
    const string draw = "draw";

    [Header("UI")]
    public TextMeshProUGUI EndGameText;

    [Header("Prefab")]
    public GameObject EndGameImage;

    [Header("References")]
    private List<HighlightTerritory> allTerritoriesHighlightScript;

    private MissionStrategyFactory MissionStrategyFactory = new MissionStrategyFactory();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        allTerritoriesHighlightScript = GameObject
            .FindGameObjectsWithTag("Territory")
            .Select(territory => territory.GetComponent<HighlightTerritory>())
            .ToList();

        playerList = GameObject.FindGameObjectsWithTag("Player").Select(PlayerDeck => PlayerDeck.GetComponent<PlayerDeck>()).ToList();

        missionCardsAvailables = new (CardDatabase.MissionCardList);
        powerCardsAvailable = new (CardDatabase.PowerCardsList);

        foreach (var card in CardDatabase.TerrainCardsList)
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
        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, playerTurn, 1f);
        UIManager.instance.SetPlayerTurnIcon(NextPlayer, playerTurn, 0f);
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
        endTurnButton.interactable = false;
        isOnBattle = true;
        UIManager.instance.SetBattlefieldBackgroundColor(territory.Type);


        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, attackTurn, 1f);
        UIManager.instance.ShowBattlefield();

        actionMade = true;
        contestedTerritory = territory;
        attack = true;
        Debug.Log("Pode escolher a carta");
        ActualPlayer.StartAttackDefenseRound();
    }

    public void SetExtraPower(int extraPowerSelected)
    {
        extraPower = extraPowerSelected == extraPower ? 0 : extraPowerSelected;

        endAttackTurnText.text = extraPower == 0 ? "Atacar sem poder extra" : $"Atacar com poder extra de {extraPower}";
    }

    public void EndAttackTurnWithExtraPowerCard()
    {
        UIManager.instance.CloseExtraPowerCardPanel();
        ActualPlayer.RemovePowerCard(extraPower);        
        
        EndAttackTurn();
    }

    public void SetAttackDefenseCard(WarriorCard card)
    {
        if (attack)
        {
            Debug.Log("Setou a carta de ataque");
            
            attackingCard = card;
            attack = false;

            UIManager.instance.ShowAttackWarriorCard(attackingCard);

            if(ActualPlayer.PowerCardsInPlayerHand.Count > 0)
            {
                UIManager.instance.ShowExtraPowerCardPanel();
            } 
            else
            {
                EndAttackTurn();
            }
        }
        else
        {
            Debug.Log("Setou a carta de defesa");
            defendindCard = card;

            UIManager.instance.ShowDefenseWarriorCard(defendindCard);

            NextPlayer.EndAttackDefenseRound();
            StartCoroutine(CalculateWinner());
        }
    }

    public void EndAttackTurn()
    {
        ActualPlayer.EndAttackDefenseRound();

        Debug.Log("Defesa pode escolher a carta");

        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, attackTurn, 0f);
        UIManager.instance.SetPlayerTurnIcon(NextPlayer, defenseTurn, 1f);

        NextPlayer.StartAttackDefenseRound();
    }

    public IEnumerator CalculateWinner()
    {
        yield return new WaitForSeconds(5f);

        var attackValue = attackingCard.GetPowerValue(contestedTerritory.Type) + extraPower;
        var defenseValue = defendindCard.GetPowerValue(contestedTerritory.Type);

        if (attackValue > defenseValue)
        {
            Debug.Log("Ataque venceu");
            UIManager.instance.SetPlayerTurnIcon(ActualPlayer, winner, 1f);
            UIManager.instance.SetPlayerTurnIcon(NextPlayer, looser, 1f);
            ActualPlayer.AddTerritory(contestedTerritory);
        }
        else if (defenseValue > attackValue)
        {
            Debug.Log("Defesa Venceu");
            UIManager.instance.SetPlayerTurnIcon(ActualPlayer, looser, 1f);
            UIManager.instance.SetPlayerTurnIcon(NextPlayer, winner, 1f);
            if (NextPlayer.GetTerritoriesWithPlayer().Contains(contestedTerritory))
            {
                int randomIndexPowerCard = Random.Range(0, powerCardsAvailable.Count);
                NextPlayer.AddNewPowerCard(powerCardsAvailable[randomIndexPowerCard]);
            }
            else
            {
                NextPlayer.AddTerritory(contestedTerritory);
            }
        }
        else {
            UIManager.instance.SetPlayerTurnIcon(ActualPlayer, draw, 1f);
            UIManager.instance.SetPlayerTurnIcon(NextPlayer, draw, 1f);
            Debug.Log("Empate");
        }
        ActualPlayer.DiscartWarriorCard(attackingCard);
        NextPlayer.DiscartWarriorCard(defendindCard);
        
        ActualPlayer.DiscartTerrainCardByType(contestedTerritory.Type);
        
        Destroy(GameObject.FindWithTag("AttackButton"));
        
        attackingCard = null;
        defendindCard = null;
        contestedTerritory = null;
        extraPower = 0;

        UIManager.instance.HideCards();
        endTurnButton.interactable = true;
        isOnBattle = false;
        if (ValidEndGame())
        {
            EndGame();
        }
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

    private bool ValidEndGame()
    {
        return GameObject.FindGameObjectsWithTag("City").Select(gameObject => gameObject.GetComponent<City>()).All(city => city.Owner != null);
    }

    private void EndGame() {
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        Instantiate(EndGameImage, canvas.transform);
        EndGameText.text = $"Player {GetWinner()} venceu";
        EndGameText.gameObject.SetActive(true);
    }

    private int GetWinner()
    {
        List<(int index, int points)> playerPoints = new();

        playerList.Select((player, index) => new { player, index })
            .ToList().ForEach(indexedPlayer =>
            {
                var finalPoints = indexedPlayer.player.GetPoints()
                + indexedPlayer.player.MissionCardsInPlayerHand.Where(mission => MissionStrategyFactory.GetMissionCardStrategy(mission.MissionType).IsComplete(indexedPlayer.player)).Sum(mission => mission.Points)
                - indexedPlayer.player.MissionCardsInPlayerHand.Where(mission => !MissionStrategyFactory.GetMissionCardStrategy(mission.MissionType).IsComplete(indexedPlayer.player)).Sum(mission => mission.Points);

                indexedPlayer.player.FinalPoints.gameObject.SetActive(true);
                indexedPlayer.player.FinalPoints.text = finalPoints.ToString();

                playerPoints.Add(new(indexedPlayer.index + 1, finalPoints));
            });

        return playerPoints.OrderByDescending(player => player.points).FirstOrDefault().index;
    }

    public void RemoveHighlightOfAllTerritories(GameObject highlightClicked)
    {
        allTerritoriesHighlightScript.ForEach(territory => {
            var isTheClickedTerritory = territory.gameObject.GetInstanceID() == highlightClicked.GetInstanceID();
            if (!isTheClickedTerritory)
            {
                territory.RemoveHighlight();
            }
        });
    }
}
