using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AttackDefense : MonoBehaviour, IPointerClickHandler
{
    public bool isClickable = false;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        bool clickWithLeftButton = eventData.button == PointerEventData.InputButton.Left;
        if (isClickable && clickWithLeftButton)
        {
            var displayWarriorCard = GetComponent<DisplayWarriorCard>();

            string cardSerialized = JsonConvert.SerializeObject(displayWarriorCard.Card);

            if (GameManager.instance.IsMyTurn())
            {
                GameManager.instance.SetAttackCard(cardSerialized);
            } 
            else
            {
                GameManager.instance.SetDefenseCard(cardSerialized);
                GameManager.instance.photonView.RPC(nameof(GameManager.instance.SetDefenseCard), Photon.Pun.RpcTarget.Others, cardSerialized);
            }
        }
    }
}
