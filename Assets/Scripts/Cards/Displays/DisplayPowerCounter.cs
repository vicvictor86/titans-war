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
        
    }

    private void Update()
    {
        UpdatePowerCounter();
    }

    public void UpdatePowerCounter()
    {
        power1CardsQuantityText.text = player.PowerCards[1].ToString();
        power2CardsQuantityText.text = player.PowerCards[2].ToString();
        power3CardsQuantityText.text = player.PowerCards[3].ToString();
        power4CardsQuantityText.text = player.PowerCards[4].ToString();
        power5CardsQuantityText.text = player.PowerCards[5].ToString();
    }
}
