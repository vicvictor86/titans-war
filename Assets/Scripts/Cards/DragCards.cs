using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCards : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    public bool IsDraggable = true;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
        {
            return;
        }

        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
        {
            return;
        }

        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
        {
            return;
        }

        var listOfHoveredGameObjects = eventData.hovered;
        bool dropZoneIsInList = false;

        foreach (var gameObject in listOfHoveredGameObjects)
        {
            var hasDropZone = gameObject.GetComponent<DropZone>();

            if (hasDropZone)
            {
                dropZoneIsInList = true;
            }
        }

        if (!dropZoneIsInList)
        {
            this.transform.SetParent(parentToReturnTo);
        }

        canvasGroup.blocksRaycasts = true;
    }
}
