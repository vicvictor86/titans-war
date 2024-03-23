using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMissionCardsScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.isMouseOverMissionCardsScroller = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.isMouseOverMissionCardsScroller = false;
    }
}
