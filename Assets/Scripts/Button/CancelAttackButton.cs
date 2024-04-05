using UnityEngine;
using UnityEngine.EventSystems;

public class CancelAttackButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ataque butao");
        GameManager.instance.CancelAttackRound();
    }

}
