using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCard : MonoBehaviour
{
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI totalForceText;
    public TextMeshProUGUI waterForceText;
    public TextMeshProUGUI desertForceText;
    public TextMeshProUGUI mountainsForceText;
    public TextMeshProUGUI plainsForceText;
    public Image cardImage;

    private void Start()
    {
        nameText.text = card.cardName;
        typeText.text = card.type;
        totalForceText.text = card.totalForce.ToString();
        waterForceText.text = card.waterForce.ToString();
        desertForceText.text = card.desertForce.ToString();
        mountainsForceText.text = card.mountainsForce.ToString();
        plainsForceText.text = card.plainsForce.ToString();
        cardImage.sprite = card.cardImage;
    }
}
