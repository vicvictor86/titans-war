using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI riverCardsQuantityText;
    [SerializeField] private TextMeshProUGUI mountainCardsQuantityText;
    [SerializeField] private TextMeshProUGUI plainsCardsQuantityText;
    [SerializeField] private TextMeshProUGUI desertCardsQuantityText;

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
    }
}
