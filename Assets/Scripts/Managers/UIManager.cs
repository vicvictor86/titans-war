using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPun
{
    public static UIManager instance;
    public GameObject waitingScreen;

    [Header("Icons")]
    [SerializeField] private GameObject turnIcon;
    [SerializeField] private Dictionary<string, Sprite> iconsAvailable;

    [Header("Mission Cards")]
    [SerializeField] private GameObject firstRoundGameObject;
    [SerializeField] private GameObject missionCardsToChoosePanel;
    [SerializeField] private GameObject playerMissionCards;
    [SerializeField] public GameObject playerMissionCardsContent;
    public bool isMouseOverMissionCardsScroller = false;

    private List<MissionCard> missionCardsInScroller = new();

    [Header("Prefabs")]
    [SerializeField] private GameObject missionCardPrefab;

    [Header("References")]
    [SerializeField] private GameObject battlefieldGameObject;
    [SerializeField] private TextMeshProUGUI battlefieldCityText;
    [SerializeField] private TextMeshProUGUI battlefieldTerrainText;
    [SerializeField] private TextMeshProUGUI battlefieldPointText;
    [SerializeField] private Image battlefieldBackground;
    [SerializeField] private GameObject chooseExtraPowerCardPanel;
    [SerializeField] private Animator panelGlowAnimator;

    [Header("Images")]
    [SerializeField] private Sprite DesertImage;
    [SerializeField] private Sprite MountainsImage;
    [SerializeField] private Sprite RiverImage;
    [SerializeField] private Sprite PlainsImage;

    [Header("Related Scripts")]
    [SerializeField] private BattlefieldUI battlefieldUI;

    private Dictionary<TerrainType, (string terrainDescription, Sprite terrainImage)> terrainInfo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        iconsAvailable = Resources.LoadAll<Sprite>("Sprites/UI/Round").ToDictionary(sprite => sprite.name, sprite => sprite);
        terrainInfo = new()
        {
            { TerrainType.MOUNTAINS, ("Montanhas", MountainsImage) },
            { TerrainType.RIVER, ("Rio", RiverImage) },
            { TerrainType.DESERT, ("Deserto", DesertImage) },
            { TerrainType.PLAINS, ("Planície", PlainsImage) },
        };

        waitingScreen.SetActive(true);
        photonView.RPC(nameof(StartGame), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            waitingScreen.SetActive(false);
        }
    }

    public void UpdateMissionCardsScroller(MissionCard missionCard)
    {
        var cardInstance = Instantiate(missionCardPrefab, playerMissionCardsContent.transform);
        cardInstance.GetComponent<DisplayMissionCard>().Card = missionCard;
        missionCardsInScroller.Add(missionCard);
    }

    private void Update()
    {
        if (GameManager.instance.myPlayer.isAlreadySelectedMissionCard)
        {
            var missionCardsInPlayerHand = GameManager.instance.myPlayer.MissionCardsInPlayerHand;
            if (missionCardsInScroller.Count != missionCardsInPlayerHand.Count)
            {
                missionCardsInPlayerHand
                    .Where(playerCard => !missionCardsInScroller
                    .Any(scrollerCard => scrollerCard.Description == playerCard.Description))
                    .ToList()
                    .ForEach(playerCard =>
                    {
                        var cardInstance = Instantiate(missionCardPrefab, playerMissionCardsContent.transform);
                        cardInstance.GetComponent<DisplayMissionCard>().Card = playerCard;
                        missionCardsInScroller.Add(playerCard);
                    });
            }

            panelGlowAnimator.SetBool("CandEndTurn", GameManager.instance.actionMade && !GameManager.instance.isOnBattle);
        }
    }

    public void OpenMissionCardsToChoosePanel(List<MissionCard> missionCardsInPlayerHand)
    {
        GameManager.instance.missionCardsToChoose.Clear();

        firstRoundGameObject.SetActive(true);

        int missionCardInPlayerHandToSelectIndex = 0;
        foreach (Transform missionCard in missionCardsToChoosePanel.transform)
        {
            var missionCardInPlayerHandSelected = missionCardsInPlayerHand[missionCardInPlayerHandToSelectIndex];
            var childDisplayMissionCard = missionCard.GetComponentInChildren<DisplayMissionCard>();
            childDisplayMissionCard.selectedBorder.SetActive(false);

            childDisplayMissionCard.Card = missionCardInPlayerHandSelected;
            childDisplayMissionCard.DescriptionText.text = missionCardInPlayerHandSelected.Description;
            childDisplayMissionCard.Points.text = missionCardInPlayerHandSelected.Points.ToString();
            childDisplayMissionCard.NameText.text = missionCardInPlayerHandSelected.Description;
            childDisplayMissionCard.CardImage.sprite = missionCardInPlayerHandSelected.CardImage;
            childDisplayMissionCard.IsSelected = false;

            missionCardInPlayerHandToSelectIndex++;

            GameManager.instance.missionCardsToChoose.Add(childDisplayMissionCard);
        }
    }

    public void CloseMissionCardsToChoosePanel()
    {
        GameManager.instance.missionCardsToChoose.Clear();

        Destroy(firstRoundGameObject.gameObject);
    }

    public void SetPlayerTurnIcon(PlayerDeck actualPlayer, string iconName, float alpha)
    {
        actualPlayer.turnIcon.color = new Color(1f, 1f, 1f, alpha);
        actualPlayer.turnIcon.sprite = iconsAvailable[iconName];
    }

    public void ShowBattlefield()
    {
        battlefieldGameObject.SetActive(true);
    }

    public void ShowAttackWarriorCard(WarriorCard warriorCard)
    {
        battlefieldUI.ShowAttackWarriorCard(warriorCard);
    }

    public void ShowDefenseWarriorCard(WarriorCard warriorCard)
    {
        battlefieldUI.ShowDefenseWarriorCard(warriorCard);
    }

    public void HideCards()
    {
        battlefieldGameObject.SetActive(false);
        battlefieldUI.HideCards();
    }

    public void ShowExtraPowerCardPanel()
    {
        chooseExtraPowerCardPanel.SetActive(true);
        chooseExtraPowerCardPanel.GetComponentsInChildren<DisplayPowerCard>()
            .ToList()
            .ForEach(card => card.Player = GameManager.instance.ActualPlayer);
        var displayPowerCounter = chooseExtraPowerCardPanel.GetComponentInChildren<DisplayPowerCounter>();
        displayPowerCounter.Player = GameManager.instance.ActualPlayer;
        displayPowerCounter.UpdatePowerCounter();
    }

    public void CloseExtraPowerCardPanel()
    {
        chooseExtraPowerCardPanel.SetActive(false);
    }

    public void SetBattlefieldBackgroundColor(TerrainType terrainType)
    {
        battlefieldBackground.color = TerrainColorConstants.colors[terrainType];
    }

    public void SetBattleFieldData(Territory territory)
    {
        battlefieldCityText.text = territory.City.name;
        battlefieldPointText.text = territory.Point.ToString();
        battlefieldTerrainText.text = terrainInfo[territory.Type].terrainDescription;

        battlefieldBackground.sprite = terrainInfo[territory.Type].terrainImage;
    }
}
