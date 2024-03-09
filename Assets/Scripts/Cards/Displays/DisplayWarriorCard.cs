using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayWarriorCard : DisplayCard<WarriorCard>, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI TypeText;
    public TextMeshProUGUI TotalForceText;
    public TextMeshProUGUI WaterForceText;
    public TextMeshProUGUI DesertForceText;
    public TextMeshProUGUI MountainsForceText;
    public TextMeshProUGUI PlainsForceText;

    private Animator anim;
    private Transform parentToReturnTo = null;
    private int siblingIndex;
    private bool isOverable = true;

    protected override void Start()
    {
        base.Start();
        TypeText.text = Card.Nationality;
        TotalForceText.text = Card.TotalForce.ToString();
        WaterForceText.text = Card.WaterForce.ToString();
        DesertForceText.text = Card.DesertForce.ToString();
        MountainsForceText.text = Card.MountainsForce.ToString();
        PlainsForceText.text = Card.PlainsForce.ToString();
        anim = gameObject.GetComponent<Animator>();
        parentToReturnTo = transform.parent;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOverable) {
            return;
        }
        Debug.Log("entrou");
        siblingIndex = gameObject.transform.GetSiblingIndex();
        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayWarriorCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = false;
        }
        anim.SetBool("IsMouseOver", true);
        gameObject.transform.SetParent(gameObject.transform.parent.transform.parent, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("saiu");
        anim.SetBool("IsMouseOver", false);
        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayWarriorCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = true;
        }
        gameObject.transform.SetParent(parentToReturnTo, true);
        gameObject.transform.SetSiblingIndex(siblingIndex);
    }
}
