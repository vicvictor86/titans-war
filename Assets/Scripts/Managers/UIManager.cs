using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Icons")]
    [SerializeField] private GameObject turnIcon;
    [SerializeField] private Dictionary<string, Sprite> iconsAvailable; 

    [Header("Mission Cards")]
    [SerializeField] private GameObject firstRoundGameObject;
    [SerializeField] private GameObject missionCardsToChoosePanel;
    [SerializeField] private GameObject playerMissionCards;
    [SerializeField] private GameObject playerMissionCardsContent;
    public bool isMouseOverMissionCardsScroller = false;

    private List<MissionCard> missionCardsInScroller = new();
    private bool missionCardScrollerIsOpen = false;

    [Header("Prefabs")]
    [SerializeField] private GameObject missionCardPrefab;

    [Header("References")]
    [SerializeField] private GameObject battlefieldGameObject;
    [SerializeField] private Image battlefieldBackground;
    [SerializeField] private GameObject chooseExtraPowerCardPanel;
    [SerializeField] private Animator panelGlowAnimator;

    [Header("Related Scripts")]
    [SerializeField] private BattlefieldUI battlefieldUI;

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
    }

    private void Update()
    {
        if (missionCardScrollerIsOpen)
        {
            var missionCardsInPlayerHand = GameManager.instance.ActualPlayer.MissionCardsInPlayerHand;
            if (missionCardsInScroller.Count != missionCardsInPlayerHand.Count)
            {
                MissionCard newMissionCard = null;
                foreach(var missionCardsInScrollerActual in missionCardsInScroller)
                {
                    foreach(var missionCardInPlayerHandActualIteration in missionCardsInPlayerHand)
                    {
                        if (missionCardsInScrollerActual.Description != missionCardInPlayerHandActualIteration.Description)
                        {
                            newMissionCard = missionCardInPlayerHandActualIteration;
                        }
                    }
                }

                if (newMissionCard)
                {
                    var cardInstance = Instantiate(missionCardPrefab, playerMissionCardsContent.transform);
                    cardInstance.GetComponent<DisplayMissionCard>().Card = newMissionCard;
                    missionCardsInScroller.Add(newMissionCard);
                }
            }
        }
    
        panelGlowAnimator.SetBool("CandEndTurn", GameManager.instance.actionMade && !GameManager.instance.isOnBattle);
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

    public void HandleClickMissionCardsScroller(List<MissionCard> missionCards)
    {
        if (!missionCardScrollerIsOpen)
        {
            playerMissionCards.SetActive(true);
            foreach (var missionCard in missionCards)
            {
                var cardInstance = Instantiate(missionCardPrefab, playerMissionCardsContent.transform);
                cardInstance.GetComponent<DisplayMissionCard>().Card = missionCard;
                missionCardsInScroller.Add(missionCard);
            }
        }
        else 
        { 
            playerMissionCards.SetActive(false);
            foreach (Transform missionCardContent in playerMissionCardsContent.transform)
            {
                Destroy(missionCardContent.gameObject);
                missionCardsInScroller.Clear();
            }
        }
        missionCardScrollerIsOpen = !missionCardScrollerIsOpen;
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
    }

    public void CloseExtraPowerCardPanel()
    {
        chooseExtraPowerCardPanel.SetActive(false);
    }

    public void SetBattlefieldBackgroundColor(TerrainType terrainType)
    {
        battlefieldBackground.color = TerrainColorConstants.colors[terrainType];
    }
}
