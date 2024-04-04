using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AttackDefense : MonoBehaviour, IPointerClickHandler
{
    public bool isClickable = false;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        if (isClickable && clickWithLeftButton)
        {
            var displayWarriorCard = GetComponent<DisplayWarriorCard>();
            GameManager.instance.SetAttackDefenseCard(displayWarriorCard.Card);
        }
    }
}
