using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerClickHandler
{
    public Territory territory;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ataque butao");
        GameManager.instance.AttackRound(territory);
    }

}
