using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayMissionCardsScroller : MonoBehaviour
{
    [SerializeField] private PlayerDeck player;

    public void ShowMissionCardsScroller()
    {
        UIManager.instance.HandleClickMissionCardsScroller(player.MissionCardsInPlayerHand);
    }
}
