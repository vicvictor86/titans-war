using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPowerCounter : MonoBehaviour
{
    [SerializeField] private PlayerDeck player;

    [Header("Terrain Cards")]
    [SerializeField] private TextMeshProUGUI power1CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power2CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power3CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power4CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power5CardsQuantityText;

    private void Start()
    {
        int randomIndexPowerCard = Random.Range(0, CardDatabase.PowerCardsList.Count);
        player.AddNewPowerCard(CardDatabase.PowerCardsList[randomIndexPowerCard]);
    }

    private void Update()
    {
        UpdatePowerCounter();
    }

    public void UpdatePowerCounter()
    {
        power1CardsQuantityText.text = player.power1CardsQuantity.ToString();
        power2CardsQuantityText.text = player.power2CardsQuantity.ToString();
        power3CardsQuantityText.text = player.power3CardsQuantity.ToString();
        power4CardsQuantityText.text = player.power4CardsQuantity.ToString();
        power5CardsQuantityText.text = player.power5CardsQuantity.ToString();
    }
}
