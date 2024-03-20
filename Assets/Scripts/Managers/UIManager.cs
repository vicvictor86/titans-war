using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Mission Cards")]
    [SerializeField] private GameObject firstRoundGameObject;
    [SerializeField] private GameObject missionCardsToChoosePanel;
    [SerializeField] private GameObject playerMissionCards;
    [SerializeField] private GameObject playerMissionCardsContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject missionCardPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
            var childDisplayMissionCard = missionCard.GetComponent<DisplayMissionCard>();

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

    public void ShowMissionCardsScroller(List<MissionCard> missionCards)
    {
        playerMissionCards.SetActive(true);
        foreach(var missionCard in missionCards)
        {
            var cardInstance = Instantiate(missionCardPrefab, playerMissionCardsContent.transform);
            cardInstance.GetComponent<DisplayMissionCard>().Card = missionCard;
        }
    }
}
