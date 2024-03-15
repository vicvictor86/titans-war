using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Cards Quantity")]
    [SerializeField] private TextMeshProUGUI riverCardsQuantityText;
    [SerializeField] private TextMeshProUGUI mountainCardsQuantityText;
    [SerializeField] private TextMeshProUGUI plainsCardsQuantityText;
    [SerializeField] private TextMeshProUGUI desertCardsQuantityText;

    [Header("Stacked Cards")]
    [SerializeField] private GameObject stackedCardsContainer;

    [Header("Mission Cards")]
    [SerializeField] private GameObject firstRoundGameObject;
    [SerializeField] private GameObject missionCardsToChoosePanel;

    [Header("Player Status")]
    [SerializeField] private GameObject playerTotalPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdateTerrainCards(int riverCardsQuantity, int mountainsCardsQuantity, int plainsCardsQuantity, int desertCardsQuantity)
    {
        riverCardsQuantityText.text = riverCardsQuantity.ToString();
        mountainCardsQuantityText.text = mountainsCardsQuantity.ToString();
        plainsCardsQuantityText.text = plainsCardsQuantity.ToString();
        desertCardsQuantityText.text = desertCardsQuantity.ToString();

        foreach(Transform stackedCardContainerChild in stackedCardsContainer.transform)
        {
            int terrainTypeQuantity = 0;
            if (stackedCardContainerChild.gameObject.name.Contains("River"))
            {
                terrainTypeQuantity = riverCardsQuantity;
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Mountain"))
            {
                terrainTypeQuantity = mountainsCardsQuantity;
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Plains"))
            {
                terrainTypeQuantity = plainsCardsQuantity;
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Desert"))
            {
                terrainTypeQuantity = desertCardsQuantity;
            }

            int stackedCardQuantity = stackedCardContainerChild.transform.childCount;
            int actualStackedCard = 1;
            foreach (Transform stackedCard in stackedCardContainerChild.transform)
            {
                bool isLastStackedCardChild = actualStackedCard == stackedCardQuantity;
                if (isLastStackedCardChild)
                {
                    bool needToActivate = terrainTypeQuantity >= 1;
                    stackedCard.gameObject.SetActive(needToActivate);
                } 

                bool isBeforeLastStackedCardChild = actualStackedCard == stackedCardQuantity - 1;
                if (isBeforeLastStackedCardChild)
                {
                    bool needToActivate = terrainTypeQuantity > 10;
                    stackedCard.gameObject.SetActive(needToActivate);
                }

                bool isAntpenultStackedCardChild = actualStackedCard == stackedCardQuantity - 2;
                if (isAntpenultStackedCardChild)
                {
                    bool needToActivate = terrainTypeQuantity > 15;
                    stackedCard.gameObject.SetActive(needToActivate);
                }

                actualStackedCard++;
            }
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
            childDisplayMissionCard.isClickable = true;
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

    public void UpdatePlayerTotalPoints(int playerTotalPoints)
    {
        this.playerTotalPoints.GetComponent<TextMeshProUGUI>().text = playerTotalPoints.ToString();
    }
}
