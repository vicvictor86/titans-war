using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerDeck playerDeck;
    public List<PlayerDeck> playerList;
    public int actualPlayerIndex;

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
        playerDeck = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeck>();
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
        var actualPlayer = playerList[actualPlayerIndex];
        actualPlayer.Round();
    }

    private void PlayerEndRound()
    {
        var actualPlayer = playerList[actualPlayerIndex];
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
        return playerdDeck == playerList[actualPlayerIndex];
    }
}
