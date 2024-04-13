using Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTerrainCounter : MonoBehaviour
{
    [SerializeField] private PlayerDeck player;

    [Header("Terrain Cards")]
    [SerializeField] private TextMeshProUGUI riverCardsQuantityText;
    [SerializeField] private TextMeshProUGUI mountainCardsQuantityText;
    [SerializeField] private TextMeshProUGUI plainsCardsQuantityText;
    [SerializeField] private TextMeshProUGUI desertCardsQuantityText;
    [SerializeField] private TextMeshProUGUI jokerCardsQuantityText;

    [Header("Stacked Cards")]
    [SerializeField] private GameObject stackedCardsContainer;

    private void Update()
    {
        UpdateTerrainCards();
    }

    public void UpdateTerrainCards()
    {
        riverCardsQuantityText.text = player.TerrainCardsQuantity[TerrainType.RIVER].ToString();
        mountainCardsQuantityText.text = player.TerrainCardsQuantity[TerrainType.MOUNTAINS].ToString();
        plainsCardsQuantityText.text = player.TerrainCardsQuantity[TerrainType.PLAINS].ToString();
        desertCardsQuantityText.text = player.TerrainCardsQuantity[TerrainType.DESERT].ToString();
        jokerCardsQuantityText.text = player.TerrainCardsQuantity[TerrainType.JOKER].ToString();

        foreach (Transform stackedCardContainerChild in stackedCardsContainer.transform)
        {
            int terrainTypeQuantity = 0;
            if (stackedCardContainerChild.gameObject.name.Contains("River"))
            {
                terrainTypeQuantity = player.TerrainCardsQuantity[TerrainType.RIVER];
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Mountain"))
            {
                terrainTypeQuantity = player.TerrainCardsQuantity[TerrainType.MOUNTAINS];
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Plains"))
            {
                terrainTypeQuantity = player.TerrainCardsQuantity[TerrainType.PLAINS];
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Desert"))
            {
                terrainTypeQuantity = player.TerrainCardsQuantity[TerrainType.DESERT];
            }
            else if (stackedCardContainerChild.gameObject.name.Contains("Joker"))
            {
                terrainTypeQuantity = player.TerrainCardsQuantity[TerrainType.JOKER];
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
