using Domain;
using System.Collections;
using System.Collections.Generic;
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
        for (int i = playerDeck.WarriorsInitialQuantity; i > 0; i--)
        {
            TerrainType terrainSelected = TerrainCardDeck.SelectRandomTerrainType();

            AddTerrainCardToHand(terrainSelected, playerDeck);
        }
    }

    public void AddTerrainCardToHand(TerrainType terrainType, PlayerDeck playerDeck)
    {
        GameManager.instance.terrainCardsAvailable[terrainType].quantityCard--;
        playerDeck.TerrainCardsInPlayerHand.Add(GameManager.instance.terrainCardsAvailable[terrainType].terrainCard);

        switch (terrainType)
        {
            case TerrainType.RIVER:
                Debug.Log("Novo river");
                playerDeck.RiverCardsQuantity++;
                break;
            case TerrainType.MOUNTAINS:
                Debug.Log("Novo mountain");
                playerDeck.MountainCardsQuantity++;
                break;
            case TerrainType.PLAINS:
                Debug.Log("Novo plains");
                playerDeck.PlainsCardsQuantity++;
                break;
            case TerrainType.DESERT:
                Debug.Log("Novo desert");
                playerDeck.DesertCardsQuantity++;
                break;
            default:
                break;
        }

        UIManager.instance.UpdateTerrainCards(playerDeck.RiverCardsQuantity, playerDeck.MountainCardsQuantity, playerDeck.PlainsCardsQuantity, playerDeck.DesertCardsQuantity);
    }

    public List<MissionCard> DrawInitialsMissionsCard(List<MissionCard> missionCardsAvailable, PlayerDeck playerDeck)
    {
        for (int i = playerDeck.MissionCardsInitialQuantity; i > 0; i--)
        {
            int randomCard = Random.Range(0, missionCardsAvailable.Count);

            var cardSelected = missionCardsAvailable[randomCard];
            playerDeck.InstantiateNewMissionCard(cardSelected);

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
        if (GameManager.instance.terrainCardsAvailable.Count <= 0)
        {
            Debug.Log("No more cards available in deck");
            return null;
        }

        TerrainType terrainSelected = TerrainCardDeck.SelectRandomTerrainType();
        AddTerrainCardToHand(terrainSelected, playerDeck);

        Debug.Log(terrainSelected);

        var cardDrawed = GameManager.instance.terrainCardsAvailable[terrainSelected].terrainCard;

        Debug.Log(cardDrawed);

        Debug.Log(GameManager.instance.terrainCardsAvailable[terrainSelected].quantityCard);

        return cardDrawed;
    }
}
