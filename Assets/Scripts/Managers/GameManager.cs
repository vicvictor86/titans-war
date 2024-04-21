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
using System;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance;
    public GameObject myPlayerSet;
    public GameObject opponentPlayerSet;

    public PlayerDeck myPlayer;
    public PlayerDeck opponentPlayer;

    //public PlayerDeck ActualPlayer => playerList[actualPlayerIndex];
    //public PlayerDeck NextPlayer => playerList[(actualPlayerIndex + 1) % playerList.Count];
    [Header("Players")]
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;
    public GameObject playerSet;
    public GameObject opponentSet;
    public GameObject MyChooseExtraPowerCardPanel;
    public int MissionCardsInitialQuantity = 6;

    [Header("Attack/Defense")]
    public bool actionMade = false;
    public WarriorCard attackingCard;
    public WarriorCard defendindCard;
    public Territory contestedTerritory = null;
    public bool attack = false;
    private int extraPower = 0;
    private string attackCardSerialized;
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

        MyChooseExtraPowerCardPanel = playerInstance.transform.Find("ChooseExtraPowerCardPanel").GameObject();

        playerInstance.transform.SetParent(playerSet.transform);
        playerInstance.transform.localScale = new Vector3(1, 1, 1);

        playerSet.transform.SetSiblingIndex(1);

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

            var cities = GameObject.FindGameObjectsWithTag("City").Select(gameObject => gameObject.GetComponent<City>());

            var startMap = new StartMap();

            foreach (var city in cities)
            {
                city.StartMap();
                startMap.startTerritoriesData.AddRange(city.TerritoriesGameObject.Select(territory =>
                {
                    var territoryComponent = territory.GetComponent<Territory>();
                    return new StartTerritoryData
                    {
                        posx = territory.transform.position.x,
                        posy = territory.transform.position.y,
                        CityName = city.CityName,
                        Point = territoryComponent.Point,
                        Type = territoryComponent.Type
                    };
                }).ToList());
            }

            var startMapSerialized = JsonConvert.SerializeObject(startMap);
            photonView.RPC(nameof(SendMap), RpcTarget.Others, startMapSerialized);

            SendCardsToOtherPlayer(cardsSelectedJson);
            photonView.RPC(nameof(SendCardsToOtherPlayer), RpcTarget.Others, cardsSelectedJson);

            actualPlayerIndex = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Count());
            photonView.RPC(nameof(SyncTurn), RpcTarget.OthersBuffered, actualPlayerIndex);
        }
    }

    [PunRPC]
    private void SendMap(string serializedMapData)
    {
        var startMap = JsonConvert.DeserializeObject<StartMap>(serializedMapData);

        var territorySprites = Resources.LoadAll<Sprite>("Sprites/HexTerrains").ToList();

        Dictionary<TerrainType, Sprite> spritesByName = new()
        {
            { TerrainType.DESERT, territorySprites[0] },
            { TerrainType.MOUNTAINS, territorySprites[1] },
            { TerrainType.PLAINS, territorySprites[2] },
            { TerrainType.RIVER, territorySprites[3] },
        };

        GameObject.FindGameObjectsWithTag("City").ToList().ForEach(cityObject =>
        {
            var city = cityObject.GetComponent<City>();

            List<TerrainType> typesAvailable = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();
            typesAvailable.RemoveAll(terrainType => terrainType == TerrainType.JOKER);

            var startTerritoryInCity = startMap.startTerritoriesData.Where(startTerritoryData => startTerritoryData.CityName == city.CityName);

            city.TerritoriesGameObject.ForEach(territoryObject =>
            {
                var territory = territoryObject.GetComponent<Territory>();

                var startTerritory = startMap.startTerritoriesData
                .FirstOrDefault(startTerritoryData =>
                    startTerritoryData.posx == territoryObject.transform.position.x &&
                    startTerritoryData.posy == territoryObject.transform.position.y
                );

                var territoryPointText = territory.GetComponentInChildren<TextMeshPro>();
                var territorySprite = territory.GetComponent<SpriteRenderer>();

                territory.Point = startTerritory.Point;
                territory.Type = startTerritory.Type;

                city.ChooseTerritoryPoint(territoryPointText, territory, territory.Point);

                city.ChooseTerritoryType(typesAvailable, spritesByName, territorySprite, territory, territory.Type);
            });
        });
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

        TerritoryDataSerializable territoryDataSerializable = new()
        {
            Type = territory.Type,
            Point = territory.Point,
            CityName = territory.City?.CityName,
        };

        string serializedTerritory = JsonConvert.SerializeObject(territoryDataSerializable, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        IniatilizeBattle(serializedTerritory);

        Destroy(GameObject.FindWithTag("AttackButton"));
        Instantiate(CancelAttackButton, attackButtonPosition.position, Quaternion.identity, canvas.transform);

        actionMade = true;
        contestedTerritory = territory;
        photonView.RPC(nameof(SetContestedTerritory), RpcTarget.Others, territory.transform.position.x, territory.transform.position.y);

        attack = true;
        Debug.Log("Pode escolher a carta");
    }

    [PunRPC]
    public void SetContestedTerritory(float posx, float posy)
    {
        var territoryGameObject = GameObject.FindGameObjectsWithTag("Territory").FirstOrDefault(territoryObject => territoryObject.transform.position.x == posx &&
            territoryObject.transform.position.y == posy
        );

        var territory = territoryGameObject.GetComponent<Territory>();

        contestedTerritory = territory;
    }

    [PunRPC]
    public void IniatilizeBattle(string serializedTerritory, string serializedWarriorCard = null)
    {
        var territoryData = JsonConvert.DeserializeObject<TerritoryDataSerializable>(serializedTerritory);

        Territory territory = new()
        {
            Type = territoryData.Type,
            Point = territoryData.Point,
            City = territoryData.CityName != null ? GameObject.Find(territoryData.CityName).GetComponent<City>() : null,
            Owner = territoryData.OwnerPlayerSide != null ? GameObject.Find(territoryData.OwnerPlayerSide).GetComponent<PlayerDeck>() : null
        };

        if (serializedWarriorCard != null)
        {
            attackingCard = JsonConvert.DeserializeObject<WarriorCard>(serializedWarriorCard);
            attackingCard.CardImage = Resources.Load<Sprite>($"Sprites/{opponentPlayer.PlayerSide}/{attackingCard.CardName}");
            UIManager.instance.ShowAttackWarriorCard(attackingCard);
        }

        UIManager.instance.SetBattleFieldData(territory);

        var icon = IsMyTurn() ? attackTurn : defenseTurn;

        UIManager.instance.SetPlayerTurnIcon(myPlayer, icon, 1f);
        UIManager.instance.ShowBattlefield();

        myPlayer.SetAttackDefenseCardsClickable();
    }

    public void CancelAttackRound()
    {
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();

        endTurnButton.interactable = true;
        isOnBattle = false;

        UIManager.instance.HideCards();
        UIManager.instance.SetPlayerTurnIcon(myPlayer, playerTurn, 1f);

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
        myPlayer.RemovePowerCard(extraPower);

        EndAttackTurn(attackCardSerialized);
    }

    [PunRPC]
    public void StopTimer()
    {
        TimerView.GetComponent<Timer>().TimerOn = false;
    }

    [PunRPC]
    public void SetAttackCard(string serializedCard)
    {
        attackCardSerialized = serializedCard;
        var card = JsonConvert.DeserializeObject<WarriorCard>(serializedCard);

        Debug.Log("Setou a carta de ataque");
        Destroy(GameObject.FindWithTag("CancelAttackButton"));

        StopTimer();
        photonView.RPC(nameof(StopTimer), RpcTarget.Others);

        attackingCard = card;
        attackingCard.CardImage = Resources.Load<Sprite>($"Sprites/{myPlayer.PlayerSide}/{card.CardName}");
        attack = false;

        UIManager.instance.ShowAttackWarriorCard(attackingCard);
        myPlayer.SetAttackDefenseCardsNotClickable();

        if (myPlayer.PowerCardsInPlayerHand.Count > 0)
        {
            UIManager.instance.ShowExtraPowerCardPanel();
        }
        else
        {
            EndAttackTurn(serializedCard);
        }
    }

    [PunRPC]
    public void SetDefenseCard(string serializedCard)
    {
        var card = JsonConvert.DeserializeObject<WarriorCard>(serializedCard);

        Debug.Log("Setou a carta de defesa");
        defendindCard = card;

        var defendingPlayerSide = IsMyTurn() ? opponentPlayer.PlayerSide : myPlayer.PlayerSide;

        defendindCard.CardImage = Resources.Load<Sprite>($"Sprites/{defendingPlayerSide}/{card.CardName}");

        UIManager.instance.ShowDefenseWarriorCard(defendindCard);

        myPlayer.SetAttackDefenseCardsNotClickable();

        StartCoroutine(CalculateWinner());
    }

    public void EndAttackTurn(string serializedCard)
    {
        Debug.Log("Defesa pode escolher a carta");

        UIManager.instance.SetPlayerTurnIcon(myPlayer, attackTurn, 0f);
        UIManager.instance.SetPlayerTurnIcon(opponentPlayer, defenseTurn, 1f);

        TerritoryDataSerializable territory = new()
        {
            Type = contestedTerritory.Type,
            Point = contestedTerritory.Point,
            CityName = contestedTerritory.City != null ? contestedTerritory.City.CityName : null,
        };

        string serializedTerritory = JsonConvert.SerializeObject(territory);

        photonView.RPC(nameof(IniatilizeBattle), RpcTarget.Others, serializedTerritory, serializedCard);
    }

    public IEnumerator CalculateWinner()
    {
        yield return new WaitForSeconds(1f);

        var attackValue = attackingCard.GetPowerValue(contestedTerritory.Type) + extraPower;
        var defenseValue = defendindCard.GetPowerValue(contestedTerritory.Type);

        if (attackValue > defenseValue)
        {
            Debug.Log("Ataque venceu");

            if (IsMyTurn())
            {
                UIManager.instance.SetPlayerTurnIcon(myPlayer, winner, 1f);
                UIManager.instance.SetPlayerTurnIcon(opponentPlayer, looser, 1f);
                myPlayer.AddTerritory(contestedTerritory);

                UIManager.instance.playerMissionCardsContent.GetComponentsInChildren<DisplayMissionCard>()
                .Where(displayMissionCard => MissionStrategyFactory
                .GetMissionCardStrategy(displayMissionCard.Card.MissionType)
                .IsComplete(myPlayer))
                .ToList()
                .ForEach(displayMissionCard => displayMissionCard.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f));
            }
            else
            {
                opponentPlayer.AddTerritory(contestedTerritory);
                UIManager.instance.SetPlayerTurnIcon(opponentPlayer, winner, 1f);
                UIManager.instance.SetPlayerTurnIcon(myPlayer, looser, 1f);
            }
        }
        else if (defenseValue > attackValue)
        {
            Debug.Log("Defesa Venceu");

            if (IsMyTurn())
            {
                UIManager.instance.SetPlayerTurnIcon(myPlayer, looser, 1f);
                UIManager.instance.SetPlayerTurnIcon(opponentPlayer, winner, 1f);
            }
            else
            {
                UIManager.instance.SetPlayerTurnIcon(opponentPlayer, looser, 1f);
                UIManager.instance.SetPlayerTurnIcon(myPlayer, winner, 1f);

                int randomIndexPowerCard = UnityEngine.Random.Range(0, powerCardsAvailable.Count);
                myPlayer.AddNewPowerCard(powerCardsAvailable[randomIndexPowerCard]);
            }
        }
        else
        {
            UIManager.instance.SetPlayerTurnIcon(myPlayer, draw, 1f);
            UIManager.instance.SetPlayerTurnIcon(opponentPlayer, draw, 1f);
            Debug.Log("Empate");
        }

        if (IsMyTurn())
        {
            myPlayer.DiscartWarriorCard(attackingCard);

            myPlayer.DiscartTerrainCardByType(myPlayer.TerrainCardsQuantity[contestedTerritory.Type] > 0 ?
            contestedTerritory.Type :
            TerrainType.JOKER);
        }
        else
        {
            myPlayer.DiscartWarriorCard(defendindCard);
        }

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

        if (IsMyTurn() &&
            (myPlayer.ListTerrainTypesDisponibleToAttack().Contains(terrainType) ||
            myPlayer.ListTerrainTypesDisponibleToAttack().Contains(TerrainType.JOKER)) &&
            !actionMade &&
            myPlayer.WarriorCardsInPlayerHand.Any() &&
            //NextPlayer.WarriorCardsInPlayerHand.Any() &&
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

            if (opponentPlayer != null)
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
                opponentPlayer.PlayerSide = opponentPlayerSide;

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
