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
}
