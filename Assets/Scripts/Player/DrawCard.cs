using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCard : MonoBehaviour
{
    public void DrawInitialsWarriorsCard(PlayerDeck playerDeck)
    {
        playerDeck.WarriorCardsAvailableInDeck = playerDeck.PlayerSide == "Spartha" ? CardDatabase.SparthaCardList : CardDatabase.PersaCardList;

        for (int i = playerDeck.WarriorsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, playerDeck.WarriorCardsAvailableInDeck.Count);
            playerDeck.InstantiateNewWarriorCard(playerDeck.WarriorCardsAvailableInDeck[randomCard]);

            playerDeck.WarriorCardsInPlayerHand.Add(playerDeck.WarriorCardsAvailableInDeck[randomCard]);
            playerDeck.WarriorCardsAvailableInDeck.Remove(playerDeck.WarriorCardsAvailableInDeck[randomCard]);
        }
    }

    public void DrawInitialsTerrainsCard(PlayerDeck playerDeck)
    {
        for (int i = playerDeck.terrainsInitialQuantity; i > 0; i--)
        {
            AddRandomTerrainCardToHand(playerDeck);
        }
    }

    public List<MissionCard> DrawInitialsMissionsCard(List<MissionCard> missionCardsAvailable, PlayerDeck playerDeck)
    {
        for (int i = playerDeck.MissionCardsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, missionCardsAvailable.Count);

            var cardSelected = missionCardsAvailable[randomCard];
            //playerDeck.InstantiateNewMissionCard(cardSelected);

            playerDeck.MissionCardsInPlayerHand.Add(cardSelected);
            missionCardsAvailable.Remove(missionCardsAvailable[randomCard]);
        }

        return playerDeck.MissionCardsInPlayerHand;
    }

    public WarriorCard DrawWarriorCard(PlayerDeck playerDeck)
    {
        if (playerDeck.WarriorCardsAvailableInDeck.Count <= 0)
        {
            if (playerDeck.DiscartedWarriorCards.Count > 0)
            {
                playerDeck.ResetWarriorDeck();
            }
            else
            {
                Debug.Log("No more cards available in deck");
                return null;
            }
        }

        WarriorCard cardDrawed = playerDeck.WarriorCardsAvailableInDeck[0];

        playerDeck.WarriorCardsInPlayerHand.Add(cardDrawed);
        playerDeck.WarriorCardsAvailableInDeck.RemoveAt(0);

        var warriorCardInstance = playerDeck.InstantiateNewWarriorCard(cardDrawed);
        warriorCardInstance.GetComponent<DragCards>().IsDraggable = true;

        return cardDrawed;
    }

    public TerrainCard DrawTerrainCard(PlayerDeck playerDeck)
    {
        Debug.Log($"{GameManager.instance.terrainCardsAvailable.Sum(keyValue => keyValue.Value.quantityCard)}");
        if (GameManager.instance.terrainCardsAvailable.All(keyValue => keyValue.Value.quantityCard <= 0))
        {
            Debug.Log("No more cards available in deck");
            return null;
        }

        var terrainsAvailable = GameManager.instance.terrainCardsAvailable
            .Where(keyValue => keyValue.Value.quantityCard > 0).Select(keyValue => keyValue.Value).ToList();

        var randomTerrainIndex = Random.Range(0, terrainsAvailable.Count());

        var cardDrawed = terrainsAvailable[randomTerrainIndex].terrainCard;

        AddTerrainCardToHand(playerDeck, cardDrawed);

        GameManager.instance.terrainCardsAvailable[cardDrawed.Type].quantityCard--;

        return cardDrawed;
    }

    public void AddRandomTerrainCardToHand(PlayerDeck playerDeck)
    {
        var terrainCard = TerrainCardDeck.SelectRandomTerrainCard();
        AddTerrainCardToHand(playerDeck, terrainCard);
    }

    public void AddTerrainCardToHand(PlayerDeck playerDeck, TerrainCard terrainCard)
    { 
        playerDeck.TerrainCardsInPlayerHand.Add(terrainCard);
        playerDeck.TerrainCardsQuantity[terrainCard.Type]++;
    }
}
