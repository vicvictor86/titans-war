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
using Photon.Realtime;
using Newtonsoft.Json;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance;
    public GameObject myPlayerSet;
    public GameObject opponentPlayerSet;

    public PlayerDeck myPlayer;
    public PlayerDeck opponentPlayer;

    public PlayerDeck ActualPlayer => playerList[actualPlayerIndex];
    public PlayerDeck NextPlayer => playerList[(actualPlayerIndex + 1) % playerList.Count];
    [Header("Players")]
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;
    public GameObject playerSet;
    public GameObject opponentSet;
    public int MissionCardsInitialQuantity = 6;

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

    public string cardsSelectedJson = "";

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
    }

    void SpawnPlayer()
    {
        GameObject playerInstance = PhotonNetwork.Instantiate("PlayerSet", playerSet.transform.position, Quaternion.identity);

        playerInstance.transform.SetParent(playerSet.transform);
        playerInstance.transform.localScale = new Vector3(1, 1, 1);

        playerSet.transform.SetAsFirstSibling();

        myPlayer = playerInstance.GetComponentInChildren<PlayerDeck>();
        myPlayer.PlayerSide = PhotonNetwork.PlayerList.Count() > 1 ? "Persa" : "Spartha";

        var warriorDeckPanel = playerSet.transform.GetComponentsInChildren<Transform>().Where(childrenTransform => childrenTransform.name == "WarriorDeckPanel").FirstOrDefault();
        var backCardSprite = Resources.Load<Sprite>($"Sprites/BackCard/Back{myPlayer.PlayerSide}");
        warriorDeckPanel.GetComponent<Image>().sprite = backCardSprite;

        photonView.RPC(nameof(UpdatePlayerList), RpcTarget.AllBuffered);
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

    void FirstRound()
    {
        allTerritoriesHighlightScript = GameObject
            .FindGameObjectsWithTag("Territory")
            .Select(territory => territory.GetComponent<HighlightTerritory>())
            .ToList();

        missionCardsAvailables = new(CardDatabase.MissionCardList);
        powerCardsAvailable = new(CardDatabase.PowerCardsList);

        foreach (var card in CardDatabase.TerrainCardsList)
        {
            terrainCardsAvailable.Add(card.Type, new TerrainCardDeck(card, 15));
        }

        myPlayer.DrawInitialsWarriorsCard();
        myPlayer.DrawInitialsTerrainsCard();

        if (!PhotonNetwork.IsMasterClient)
        {
            var cardsSelected = DrawCard.DrawInitialsMissionsCard(missionCardsAvailables);

            MissionCardsSerializable missionCardsSerializable = new()
            {
                missionCards = cardsSelected
            };
            cardsSelectedJson = JsonConvert.SerializeObject(missionCardsSerializable);


            SendCardsToOtherPlayer(cardsSelectedJson);
            photonView.RPC(nameof(SendCardsToOtherPlayer), RpcTarget.Others, cardsSelectedJson);

            actualPlayerIndex = Random.Range(0, PhotonNetwork.PlayerList.Count());
            photonView.RPC(nameof(SyncTurn), RpcTarget.OthersBuffered, actualPlayerIndex);
        }
    }

    [PunRPC]
    private void SendCardsToOtherPlayer(string listCards)
    {
        var missionCardsSerializable = JsonConvert.DeserializeObject<MissionCardsSerializable>(listCards);
        List<MissionCard> myCards;
        List<MissionCard> opponentCards;

        foreach (var missionCard in missionCardsSerializable.missionCards)
        {
            missionCard.CardImage = Resources.Load<Sprite>($"Sprites/Missions/{missionCard.CardName}");
        }

        if (PhotonNetwork.Equals(PhotonNetwork.PlayerList[0], PhotonNetwork.LocalPlayer))
        {
            myCards = missionCardsSerializable.missionCards.Take(3).ToList();
            opponentCards = missionCardsSerializable.missionCards.Skip(3).ToList();
        }
        else
        {
            myCards = missionCardsSerializable.missionCards.Skip(3).ToList();
            opponentCards = missionCardsSerializable.missionCards.Take(3).ToList();
        }

        myPlayer.MissionCardsInPlayerHand = myCards;
        opponentPlayer.MissionCardsInPlayerHand = opponentCards;

        UIManager.instance.OpenMissionCardsToChoosePanel(myPlayer.MissionCardsInPlayerHand);
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
                myPlayer.DiscartMissionCard(displayMissionCardActual.Card);
            }

            displayMissionCardActual.isClickable = false;
        }

        myPlayer.isAlreadySelectedMissionCard = true;

        UIManager.instance.CloseMissionCardsToChoosePanel();
        UIManager.instance.waitingScreen.GetComponentInChildren<TMP_Text>().text = "Esperando o outro jogador escolher";
        UIManager.instance.waitingScreen.SetActive(true);

        photonView.RPC(nameof(SyncMissionCardSelection), RpcTarget.All);
    }

    [PunRPC]
    public void SyncMissionCardSelection()
    {
        if (AllPlayersSelectedMissionCard())
        {
            photonView.RPC(nameof(EndFirstRound), RpcTarget.All);
        }
    }

    public bool AllPlayersSelectedMissionCard()
    {
        if (PhotonNetwork.PlayerList.Count() < 2 || !opponentPlayer.isAlreadySelectedMissionCard)
        {
            return false;
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

        UIManager.instance.SetPlayerTurnIcon(myPlayer, playerTurn, isMyTurn ? 1f : 0f);
        UIManager.instance.SetPlayerTurnIcon(opponentPlayer, playerTurn, isMyTurn ? 0f : 1f);
    }

    [PunRPC]
    public void EndTurn()
    {
        if (IsMyTurn())
        {
            int nextPlayer = (actualPlayerIndex + 1) % PhotonNetwork.PlayerList.Count();

            photonView.RPC(nameof(SyncTurn), RpcTarget.AllBuffered, nextPlayer);

            PlayerRound();
            photonView.RPC(nameof(PlayerRound), RpcTarget.Others);
        }
        else
        {
            Debug.Log("Não é sua vez");
        }
    }

    public bool CanDrawCard(PlayerDeck playerdDeck)
    {
        return !actionMade && IsMyTurn();
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
        else
        {
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

    private void EndGame()
    {
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
        allTerritoriesHighlightScript.ForEach(territory =>
        {
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
            stream.SendNext(actualPlayerIndex);
            stream.SendNext(myPlayer.isAlreadySelectedMissionCard);
            stream.SendNext(myPlayer.PlayerSide);
        }
        else if (stream.IsReading)
        {
            actualPlayerIndex = (int)stream.ReceiveNext();

            if (opponentPlayer)
            {
                opponentPlayer.isAlreadySelectedMissionCard = (bool)stream.ReceiveNext();
                opponentPlayer.PlayerSide = (string)stream.ReceiveNext();
            }
        }
    }

    [PunRPC]
    void UpdatePlayerList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PhotonView>().IsMine)
            {
                opponentPlayerSet = player;
                opponentPlayer = player.GetComponentInChildren<PlayerDeck>();

                var warriorDeckPanel = opponentPlayerSet.transform.GetComponentsInChildren<Transform>().Where(childrenTransform => childrenTransform.name == "WarriorDeckPanel").FirstOrDefault();
                var opponentPlayerSide = myPlayer.PlayerSide == "Persa" ? "Spartha" : "Persa";
                var backCardSprite = Resources.Load<Sprite>($"Sprites/BackCard/Back{opponentPlayerSide}");
                warriorDeckPanel.GetComponent<Image>().sprite = backCardSprite;

                opponentPlayer.transform.parent.transform.SetParent(opponentSet.transform);
                opponentPlayer.transform.localPosition = opponentSet.transform.localPosition;
                opponentPlayer.transform.localScale = opponentSet.transform.localScale;

                opponentPlayer.transform.parent.transform.SetAsFirstSibling();
            }
            else
            {
                myPlayerSet = player;
            }
        }

        if (players.Length >= 2)
        {
            FirstRound();
        }
    }
}
