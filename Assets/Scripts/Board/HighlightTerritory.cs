using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightTerritory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entrou");
        if (eventData.pointerEnter.tag == "Territory")
        {
            eventData.pointerEnter.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    private void OnMouseOver()
    {
        Debug.Log("Entrou");
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicou");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Territory")
        {
            eventData.pointerEnter.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
