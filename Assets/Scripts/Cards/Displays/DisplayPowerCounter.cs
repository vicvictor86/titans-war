using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPowerCounter : MonoBehaviour
{
    [SerializeField] public PlayerDeck Player;

    [Header("Power Cards")]
    [SerializeField] private TextMeshProUGUI power1CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power2CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power3CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power4CardsQuantityText;
    [SerializeField] private TextMeshProUGUI power5CardsQuantityText;

    public void UpdatePowerCounter()
    {
        power1CardsQuantityText.text = Player.PowerCards[1].ToString();
        power2CardsQuantityText.text = Player.PowerCards[2].ToString();
        power3CardsQuantityText.text = Player.PowerCards[3].ToString();
        power4CardsQuantityText.text = Player.PowerCards[4].ToString();
        power5CardsQuantityText.text = Player.PowerCards[5].ToString();
    }
}
