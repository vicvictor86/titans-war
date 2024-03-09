using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerDeck actualPlayer;
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;
    public bool actionMade = false;
    public WarriorCard attackingCard;
    public WarriorCard defendindCard;
    public Territory contestedTerritory = null;
    public bool attack = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        actualPlayerIndex = 0;
        actualPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeck>();
        playerList = GameObject.FindGameObjectsWithTag("Player").Select(PlayerDeck => PlayerDeck.GetComponent<PlayerDeck>()).ToList();

        foreach (var player in playerList)
        {
            player.DrawInitialsWarriorsCard();
            player.DrawInitialsTerrainsCard();
        }

        PlayerRound();
    }

    void Update()
    {
        
    }

    private void PlayerRound()
    {
        actualPlayer = playerList[actualPlayerIndex];
        actionMade = false;
        actualPlayer.Round();
    }

    private void PlayerEndRound()
    {
        actualPlayer = playerList[actualPlayerIndex];
        actualPlayer.EndRound();
    }

    public void EndTurn()
    {
        PlayerEndRound();

        actualPlayerIndex = (actualPlayerIndex + 1) % playerList.Count;

        PlayerRound();
    }

    public bool CanDrawCard(PlayerDeck playerdDeck)
    {
        var canDrawCard = !actionMade && playerdDeck == playerList[actualPlayerIndex];
        actionMade = true;
        return canDrawCard;
    }

    public void AttackRound(Territory territory)
    {
        actionMade = true;
        contestedTerritory = territory;
        attack = true;
        Debug.Log("Pode escolher a carta");
        actualPlayer.StartAttackDefenseRound();
    }

    public void SetAttackDefenseCard(WarriorCard card)
    {
        if (attack)
        {
            Debug.Log("Setou a carta de ataque");
            attackingCard = card;
            attack = false;
            actualPlayer.EndAttackDefenseRound();
            Debug.Log("Defesa pode escolher a carta");
            playerList[(actualPlayerIndex + 1) % playerList.Count].StartAttackDefenseRound();
        }
        else
        {
            Debug.Log("Setou a carta de defesa");
            defendindCard = card;
            playerList[(actualPlayerIndex + 1) % playerList.Count].EndAttackDefenseRound();
            CalculateWinner();
        }
    }

    public void CalculateWinner()
    {
        var attackValue = attackingCard.GetPowerValue(contestedTerritory.Type);
        var defenseValue = defendindCard.GetPowerValue(contestedTerritory.Type);
        if (attackValue > defenseValue)
        {
            Debug.Log("Ataque venceu");
            actualPlayer.AddTerritory(contestedTerritory);
        }
        else if (defenseValue > attackValue)
        {
            Debug.Log("Defesa Venceu");
            if (playerList[(actualPlayerIndex + 1) % playerList.Count].GetTerritoriesWithPlayer().Contains(contestedTerritory))
            {
                //Adicionar carta de poder
            }
            else
            {
                playerList[(actualPlayerIndex + 1) % playerList.Count].AddTerritory(contestedTerritory);
            }
        }
        else {
            Debug.Log("Empate");
        }
        actualPlayer.DiscartWarriorCard(attackingCard);
        playerList[(actualPlayerIndex + 1) % playerList.Count].DiscartWarriorCard(defendindCard);
        actualPlayer.DiscartTerrainCardByType(contestedTerritory.Type);
        attackingCard = null;
        defendindCard = null;
        contestedTerritory = null;
    }
}
