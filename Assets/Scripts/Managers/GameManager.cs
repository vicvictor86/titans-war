using Assets.Scripts.Cards.Missions;
using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance;

    public PlayerDeck ActualPlayer => playerList[actualPlayerIndex];
    public PlayerDeck NextPlayer => playerList[(actualPlayerIndex + 1) % playerList.Count];
    [Header("Players")]
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;
    public GameObject playerSet;
    public GameObject opponentSet;

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
    public GameObject AttackButton;
    public GameObject CancelAttackButton;
    public GameObject TimerView;

    [Header("References")]
    private List<HighlightTerritory> allTerritoriesHighlightScript;

    private MissionStrategyFactory MissionStrategyFactory = new MissionStrategyFactory();
    private readonly int roundTime = 30;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SpawnPlayer();
        
        allTerritoriesHighlightScript = GameObject
            .FindGameObjectsWithTag("Territory")
            .Select(territory => territory.GetComponent<HighlightTerritory>())
            .ToList();

        playerList = GameObject.FindGameObjectsWithTag("Player").Select(PlayerDeck => PlayerDeck.GetComponent<PlayerDeck>()).ToList();

        missionCardsAvailables = new (CardDatabase.MissionCardList);
        powerCardsAvailable = new (CardDatabase.PowerCardsList);

        foreach (var card in CardDatabase.TerrainCardsList)
        {
            terrainCardsAvailable.Add(card.Type, new TerrainCardDeck(card, 15));
        }

        FirstRound();
    }

    void SpawnPlayer()
    {
        if(photonView.IsMine)
        {
            var playerInstance = PhotonNetwork.Instantiate("PlayerSet", new Vector2(playerSet.transform.position.x, playerSet.transform.position.y), Quaternion.identity);
            playerInstance.transform.SetParent(playerSet.transform);
            playerInstance.transform.localScale = new Vector3(1, 1, 1);

            playerSet.transform.SetAsFirstSibling();
        } else
        {
            var playerInstance = PhotonNetwork.Instantiate("PlayerSet", new Vector2(opponentSet.transform.position.x, opponentSet.transform.position.y), Quaternion.identity);
            playerInstance.transform.SetParent(opponentSet.transform);
            playerInstance.transform.localScale = new Vector3(1, 1, 1);

            opponentSet.transform.SetAsFirstSibling();
        }
    }

    public bool IsMyTurn()
    {
        return actualPlayerIndex == PhotonNetwork.LocalPlayer.ActorNumber - 1;
    }

    [PunRPC]
    private void SyncTurn(int nextPlayer)
    {
        actualPlayerIndex = nextPlayer;
    }

    [PunRPC]
    void FirstRound()
    {
        foreach (var player in playerList)
        {
            player.DrawInitialsWarriorsCard();
            player.DrawInitialsTerrainsCard();
            player.DrawInitialsMissionsCard(missionCardsAvailables);
        }

        actualPlayerIndex = Random.Range(0, playerList.Count);
        photonView.RPC(nameof(SyncTurn), RpcTarget.AllBuffered, actualPlayerIndex);

        UIManager.instance.OpenMissionCardsToChoosePanel(ActualPlayer.MissionCardsInPlayerHand);
        ChangeDisplayMissionToClickable(missionCardsToChoose);

        needSelectMissionCard = true;
    }

    [PunRPC]
    private void ChangeDisplayMissionToClickable(List<DisplayMissionCard> displayMissionCards)
    {
        foreach (var displayMissionCardActual in displayMissionCards)
        {
            displayMissionCardActual.isClickable = true;
        }
    }

    [PunRPC]
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

<<<<<<< HEAD
        playerList[PhotonNetwork.LocalPlayer.ActorNumber - 1].isAlreadySelectedMissionCard = true;
=======
        // VAMO LA PRECISA APAGAR POR FAVOR FAZ ESSE FAVOR 
        if (playerAction == playerList.IndexOf(playerList.FirstOrDefault(player => player.PlayerSide == "Spartha")))
        {
            foreach (var missionCard in playerList.FirstOrDefault(player => player.PlayerSide == "Spartha").MissionCardsInPlayerHand)
            {
                UIManager.instance.UpdateMissionCardsScroller(missionCard);
            }
        }

        playerAction++;
>>>>>>> origin/main

        UIManager.instance.CloseMissionCardsToChoosePanel();
        UIManager.instance.waitingScreen.GetComponentInChildren<TMP_Text>().text = "Esperando o outro jogador escolher";
        UIManager.instance.waitingScreen.SetActive(true);

        photonView.RPC(nameof(SyncMissionCardSelection), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);
    }

    [PunRPC]
    public void SyncMissionCardSelection(int actorNumber, bool isAlreadySelected)
    {
        playerList[actorNumber - 1].isAlreadySelectedMissionCard = isAlreadySelected;

        if (AllPlayersSelectedMissionCard())
        {
            photonView.RPC(nameof(EndFirstRound), RpcTarget.All);
        }
    }

    public bool AllPlayersSelectedMissionCard()
    {
        foreach (var player in playerList)
        {
            if (!player.isAlreadySelectedMissionCard)
            {
                return false;
            }
        }

        return true;
    }

    [PunRPC]
    private void EndFirstRound()
    {
        UIManager.instance.waitingScreen.SetActive(false);

        PlayerRound();
        photonView.RPC(nameof(PlayerRound), RpcTarget.Others);
    }

    [PunRPC]
    private void PlayerRound()
    {
        actionMade = false;
        var timer = TimerView.GetComponent<Timer>();
        timer.TimeLeft = roundTime;
        timer.TimerOn = true;

        bool isMyTurn = IsMyTurn();

        UIManager.instance.SetPlayerTurnIcon(playerList[PhotonNetwork.LocalPlayer.ActorNumber - 1], playerTurn, isMyTurn ? 1f : 0f);
        UIManager.instance.SetPlayerTurnIcon(playerList[PhotonNetwork.PlayerListOthers[0].ActorNumber - 1], playerTurn, isMyTurn ? 0f : 1f);
    }

    [PunRPC]
    public void EndTurn()
    {
        if (IsMyTurn())
        {
            int nextPlayer = (actualPlayerIndex + 1) % playerList.Count;

            photonView.RPC(nameof(SyncTurn), RpcTarget.AllBuffered, nextPlayer);

            PlayerRound();
            photonView.RPC(nameof(PlayerRound), RpcTarget.Others);
        } else
        {
            Debug.Log("Não é sua vez");
        }
    }

    public bool CanDrawCard(PlayerDeck playerdDeck)
    {
        var canDrawCard = !actionMade && playerdDeck == playerList[actualPlayerIndex];
        return canDrawCard;
    }

    public void AttackRound(Territory territory)
    {
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();

        endTurnButton.interactable = false;
        isOnBattle = true;

        UIManager.instance.SetBattleFieldData(territory);
        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, attackTurn, 1f);
        UIManager.instance.ShowBattlefield();
        Destroy(GameObject.FindWithTag("AttackButton"));
        Instantiate(CancelAttackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);

        actionMade = true;
        contestedTerritory = territory;
        attack = true;
        Debug.Log("Pode escolher a carta");
        ActualPlayer.SetAttackDefenseCardsClickable();
    }

    public void CancelAttackRound()
    {
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();

        endTurnButton.interactable = true;
        isOnBattle = false;

        UIManager.instance.HideCards();
        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, playerTurn, 1f);
        Destroy(GameObject.FindWithTag("CancelAttackButton"));
        var attackButton = Instantiate(AttackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);
        attackButton.GetComponent<AttackButton>().territory = contestedTerritory;

        actionMade = false;
        contestedTerritory = null;
        attack = false;
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
            Destroy(GameObject.FindWithTag("CancelAttackButton"));
            TimerView.GetComponent<Timer>().TimerOn = false;

            attackingCard = card;
            attack = false;

            UIManager.instance.ShowAttackWarriorCard(attackingCard);
            ActualPlayer.SetAttackDefenseCardsNotClickable();

            if (ActualPlayer.PowerCardsInPlayerHand.Count > 0)
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

            NextPlayer.SetAttackDefenseCardsNotClickable();
            StartCoroutine(CalculateWinner());
        }
    }

    public void EndAttackTurn()
    {
        Debug.Log("Defesa pode escolher a carta");

        UIManager.instance.SetPlayerTurnIcon(ActualPlayer, attackTurn, 0f);
        UIManager.instance.SetPlayerTurnIcon(NextPlayer, defenseTurn, 1f);

        NextPlayer.SetAttackDefenseCardsClickable();
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

            UIManager.instance.playerMissionCardsContent.GetComponentsInChildren<DisplayMissionCard>()
                .Where(displayMissionCard => MissionStrategyFactory
                .GetMissionCardStrategy(displayMissionCard.Card.MissionType)
                .IsComplete(ActualPlayer))
                .ToList()
                .ForEach(displayMissionCard => displayMissionCard.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
        else if (defenseValue > attackValue)
        {
            Debug.Log("Defesa Venceu");
            UIManager.instance.SetPlayerTurnIcon(ActualPlayer, looser, 1f);
            UIManager.instance.SetPlayerTurnIcon(NextPlayer, winner, 1f);
            int randomIndexPowerCard = Random.Range(0, powerCardsAvailable.Count);
            NextPlayer.AddNewPowerCard(powerCardsAvailable[randomIndexPowerCard]);
        }
        else {
            UIManager.instance.SetPlayerTurnIcon(ActualPlayer, draw, 1f);
            UIManager.instance.SetPlayerTurnIcon(NextPlayer, draw, 1f);
            Debug.Log("Empate");
        }
        ActualPlayer.DiscartWarriorCard(attackingCard);
        NextPlayer.DiscartWarriorCard(defendindCard);
        
        ActualPlayer.DiscartTerrainCardByType(ActualPlayer.TerrainCardsQuantity[contestedTerritory.Type] > 0 ?
            contestedTerritory.Type :
            TerrainType.JOKER);

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
        if ((ActualPlayer.ListTerrainTypesDisponibleToAttack().Contains(terrainType)
            || ActualPlayer.ListTerrainTypesDisponibleToAttack().Contains(TerrainType.JOKER)) &&
            !actionMade &&
            ActualPlayer.WarriorCardsInPlayerHand.Any() &&
            NextPlayer.WarriorCardsInPlayerHand.Any() &&
            territory.Owner == null)
        {
            var attackButton = Instantiate(AttackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerList[PhotonNetwork.LocalPlayer.ActorNumber - 1].isAlreadySelectedMissionCard);
        }
        else
        {
            playerList[PhotonNetwork.PlayerListOthers[0].ActorNumber - 1].isAlreadySelectedMissionCard = (bool)stream.ReceiveNext();
        }
    }
}
