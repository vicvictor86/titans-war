using Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscartCard : MonoBehaviour
{
    public void DiscartWarriorCard(WarriorCard warriorCardDiscarted, Transform playerWarriorHandPanelTransform, List<WarriorCard> warriorCardsinPlayerHands, List<WarriorCard> discartedWarriorCards)
    {
        var warriorHand = playerWarriorHandPanelTransform.GetComponentsInChildren<DisplayWarriorCard>();
        var warriorCard = warriorHand.FirstOrDefault(displayCard => displayCard.Card == warriorCardDiscarted);

        Destroy(warriorCard.gameObject);

        warriorCardsinPlayerHands.Remove(warriorCardDiscarted);
        discartedWarriorCards.Add(warriorCardDiscarted);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted, List<TerrainCard> terrainCardsinPlayerHands, PlayerDeck player)
    {
        terrainCardsinPlayerHands.Remove(terrainCardDiscarted);

        switch (terrainCardDiscarted.Type)
        {
            case TerrainType.RIVER:
                player.RiverCardsQuantity--;
                break;
            case TerrainType.MOUNTAINS:
                player.MountainCardsQuantity--;
                break;
            case TerrainType.PLAINS:
                player.PlainsCardsQuantity--;
                break;
            case TerrainType.DESERT:
                player.DesertCardsQuantity--;
                break;
            default:
                break;
        }

        UIManager.instance.UpdateTerrainCards(player.RiverCardsQuantity, player.MountainCardsQuantity, player.PlainsCardsQuantity, player.DesertCardsQuantity);
    }

    public void DiscartMissionCard(MissionCard missionCardToDiscart, Transform missionCardPlace, List<MissionCard> missionCardsinPlayerHands)
    {
        var missionHand = missionCardPlace.GetComponentsInChildren<DisplayMissionCard>();
        var missionCard = missionHand.FirstOrDefault(displayCard => displayCard.Card == missionCardToDiscart);

        Destroy(missionCard.gameObject);

        missionCardsinPlayerHands.Remove(missionCardToDiscart);
        GameManager.instance.missionCardsAvailables.Add(missionCardToDiscart);
    }

    public void DiscartTerrainCardByType(TerrainType type, Transform playerTerrainHandPanelTransform, List<TerrainCard> terrainCardsinPlayerHands, PlayerDeck player)
    {
        var terrainHand = playerTerrainHandPanelTransform.GetComponentsInChildren<DisplayTerrainCard>();
        var terrainCard = terrainHand.FirstOrDefault(displayCard => displayCard.Card.Type == type);

        DiscartTerrainCard(terrainCard.Card, terrainCardsinPlayerHands, player);
    }
}
