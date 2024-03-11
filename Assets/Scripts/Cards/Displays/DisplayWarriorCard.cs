using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayWarriorCard : DisplayCard<WarriorCard>, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI TypeText;
    public TextMeshProUGUI TotalForceText;
    public TextMeshProUGUI WaterForceText;
    public TextMeshProUGUI DesertForceText;
    public TextMeshProUGUI MountainsForceText;
    public TextMeshProUGUI PlainsForceText;

    private Animator anim;
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOverable) {
            return;
        }

        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayWarriorCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = false;
        }
        anim.SetBool("IsMouseOver", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var siblings = gameObject.transform.parent.GetComponentsInChildren<DisplayWarriorCard>();
        foreach (var sibling in siblings)
        {
            sibling.isOverable = true;
        }
        anim.SetBool("IsMouseOver", false);
    }
}
