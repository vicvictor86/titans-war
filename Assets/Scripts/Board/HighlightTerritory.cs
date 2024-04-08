using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightTerritory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Color previousColor;
    private Color highlightColor = new Color(0.9f, 0, 0.7f, 1f);
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
        if (eventData.pointerEnter.CompareTag("Territory") && !wasClicked)
        {
            var spriteRender = eventData.pointerEnter.GetComponent<SpriteRenderer>();
            previousColor = spriteRender.color != highlightColor ? spriteRender.color : previousColor;
            spriteRender.color = highlightColor;
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

    public void RemoveHighlight()
    {
        wasClicked = false;
        gameObject.GetComponent<SpriteRenderer>().color = previousColor;
    }

    public void SetDominated(Color color)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        previousColor = color;
    }
}
