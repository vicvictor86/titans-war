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
        var warriorCard = warriorHand.FirstOrDefault(displayCard => displayCard.Card.CardName == warriorCardDiscarted.CardName);

        Destroy(warriorCard.gameObject);

        warriorCardsinPlayerHands.RemoveAll(warriorCard => warriorCard.CardName == warriorCardDiscarted.CardName);
        discartedWarriorCards.Add(warriorCardDiscarted);
    }

    public void DiscartTerrainCard(TerrainCard terrainCardDiscarted, Transform playerTerrainHandPanelTransform, List<TerrainCard> terrainCardsinPlayerHands)
    {
        terrainCardsinPlayerHands.Remove(terrainCardsinPlayerHands.FirstOrDefault(terrainCard => terrainCard.Type == terrainCardDiscarted.Type));
    }

    public void DiscartMissionCard(MissionCard missionCardToDiscart, List<MissionCard> missionCardsinPlayerHands)
    {
        missionCardsinPlayerHands.RemoveAll(missionCard => missionCard.CardName == missionCardToDiscart.CardName);
        GameManager.instance.missionCardsAvailables.Add(missionCardToDiscart);
    }

    public void DiscartTerrainCardByType(TerrainType type, Transform playerTerrainHandPanelTransform, List<TerrainCard> terrainCardsinPlayerHands)
    {
        var terrainHand = playerTerrainHandPanelTransform.GetComponentsInChildren<DisplayTerrainCard>();
        var terrainCard = terrainHand.FirstOrDefault(displayCard => displayCard.Card.Type == type);

        DiscartTerrainCard(terrainCard.Card, playerTerrainHandPanelTransform, terrainCardsinPlayerHands);
    }
}
