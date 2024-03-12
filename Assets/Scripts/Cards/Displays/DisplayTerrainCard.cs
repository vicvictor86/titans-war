using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayTerrainCard : DisplayCard<TerrainCard>, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI TerrainTypeText;
    public TextMeshProUGUI StrengthText;

    private Animator anim;
    private bool isOverable = true;

    protected override void Start()
    {
        base.Start();
        StrengthText.text = Card.Strength.ToString();
        TerrainTypeText.text = Card.Type.ToString();
        anim = gameObject.GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOverable)
        {
            return;
        }

        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayTerrainCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = false;
        }
        anim.SetBool("IsMouseOver", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayTerrainCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = true;
        }
        anim.SetBool("IsMouseOver", false);
    }
}
