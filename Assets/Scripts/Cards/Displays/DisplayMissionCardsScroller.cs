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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
