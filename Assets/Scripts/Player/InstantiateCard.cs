using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCard : MonoBehaviour
{
    public GameObject InstantiateNewWarriorCard(WarriorCard warriorCard, GameObject warriorPrefab, Transform playerWarriorHandPanelTransform)
    {
        var cardInstance = Instantiate(warriorPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerWarriorHandPanelTransform);
        cardInstance.GetComponent<DisplayWarriorCard>().Card = warriorCard;
        cardInstance.transform.SetAsFirstSibling();

        return cardInstance;
    }

    public GameObject InstantiateNewMissionCard(MissionCard missionCard, GameObject missionPrefab, Transform missionCardPlace)
    {
        var cardInstance = Instantiate(missionPrefab, missionCardPlace.position, Quaternion.identity, missionCardPlace.transform);
        cardInstance.GetComponent<DisplayMissionCard>().Card = missionCard;
        cardInstance.transform.SetAsFirstSibling();

        return cardInstance;
    }
}
