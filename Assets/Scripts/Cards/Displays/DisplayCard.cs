using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCard<T> : MonoBehaviour where T : Card
{
    public T Card;

    public TextMeshProUGUI NameText;
    
    public Image CardImage;

    protected virtual void Start()
    {
        NameText.text = Card.CardName;
        
        CardImage.sprite = Card.CardImage;
    }
}
