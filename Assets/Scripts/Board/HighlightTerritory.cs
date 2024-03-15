using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightTerritory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        //if (eventData.pointerEnter.tag == "Territory")
        //{
        //    eventData.pointerEnter.GetComponent<MeshRenderer>().material.color = Color.red;
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (eventData.pointerEnter.tag == "Territory")
        //{
        //    eventData.pointerEnter.GetComponent<MeshRenderer>().material.color = Color.white;
        //}
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
