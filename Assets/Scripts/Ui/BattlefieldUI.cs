using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldUI : MonoBehaviour
{
    [SerializeField] private GameObject defenseWarriorCard;
    [SerializeField] private GameObject attackWarriorCard;

    private DisplayWarriorCard attackDisplayWarriorCard;
    private DisplayWarriorCard defenseDisplayWarriorCard;

    private void Awake()
    {
        attackDisplayWarriorCard = attackWarriorCard.GetComponent<DisplayWarriorCard>();
        defenseDisplayWarriorCard = defenseWarriorCard.GetComponent<DisplayWarriorCard>();
    }

    public void ShowAttackWarriorCard(WarriorCard warriorCard)
    {
        attackWarriorCard.SetActive(true);
        attackDisplayWarriorCard.Card = warriorCard;
        attackDisplayWarriorCard.UpdateInfos();
    }

    public void ShowDummyCard(string playerSide)
    {
        attackWarriorCard.SetActive(true);
        attackDisplayWarriorCard.Card = new();
        attackDisplayWarriorCard.Card.CardImage = Resources.Load<Sprite>($"Sprites/BackCard/Back{playerSide}");
        attackDisplayWarriorCard.UpdateInfos();
    }

    public void ShowDefenseWarriorCard(WarriorCard warriorCard)
    {
        defenseWarriorCard.SetActive(true);
        defenseDisplayWarriorCard.Card = warriorCard;
        defenseDisplayWarriorCard.UpdateInfos();
    }

    public void HideCards()
    {
        attackWarriorCard.SetActive(false);
        defenseWarriorCard.SetActive(false);
    }   
}
