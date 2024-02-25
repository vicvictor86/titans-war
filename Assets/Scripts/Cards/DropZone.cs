using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject objectDropped = eventData.pointerDrag;

        if (objectDropped.CompareTag("WarriorCard"))
        {
            DragCards dragWarriorCard = objectDropped.GetComponent<DragCards>();

            if (!dragWarriorCard.IsDraggable)
            {
                return;
            }

            objectDropped.transform.SetParent(transform);
            dragWarriorCard.IsDraggable = false;
        }

        if (objectDropped.CompareTag("TerrainCard"))
        {
            DragCards dragTerrainCard = objectDropped.GetComponent<DragCards>();

            if (!dragTerrainCard.IsDraggable)
            {
                return;
            }

            objectDropped.transform.SetParent(transform);
            dragTerrainCard.IsDraggable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
