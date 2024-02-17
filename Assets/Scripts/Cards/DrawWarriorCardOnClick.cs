using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawWarriorCardOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerDeck playerDeck;

    public void OnPointerClick(PointerEventData eventData)
    {
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        if (clickWithLeftButton && playerDeck != null)
        {
            playerDeck.DrawWarriorCard();
        }
    }
}
