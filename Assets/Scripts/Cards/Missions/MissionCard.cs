using Domain;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Card", menuName = "MissionCard")]
[Serializable]
public class MissionCard : Card
{
    [SerializeField] public string Description;
    [SerializeField] public int Points;
    [SerializeField] public MissionType MissionType;
}
