using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightTerritory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Color previousColor;
    private Color highlightColor = Color.red;
    public bool wasClicked = false;

    private void Start()
    {
        previousColor = GetComponent<SpriteRenderer>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            wasClicked = !wasClicked;
        }

        if (wasClicked)
        {
            GameManager.instance.RemoveHighlightOfAllTerritories(gameObject);
            var spriteRender = eventData.pointerEnter.GetComponent<SpriteRenderer>();
            previousColor = spriteRender.color != highlightColor ? spriteRender.color : previousColor;
            spriteRender.color = Color.green;
        }
        else
        {
            RemoveHighlight(eventData.pointerEnter);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Territory" && !wasClicked)
        {
            var spriteRender = eventData.pointerEnter.GetComponent<SpriteRenderer>();
            previousColor = spriteRender.color != highlightColor ? spriteRender.color : previousColor;
            spriteRender.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Territory" && !wasClicked)
        {
            RemoveHighlight(eventData.pointerEnter);
        }
    }

    public void RemoveHighlight(GameObject gameObjectInstance)
    {
        gameObjectInstance.gameObject.GetComponent<SpriteRenderer>().color = previousColor;
    }

    public void RemoveHighlight(HighlightTerritory highlightScriptComponent)
    {
        highlightScriptComponent.wasClicked = false;
        highlightScriptComponent.gameObject.GetComponent<SpriteRenderer>().color = previousColor;
    }
}
