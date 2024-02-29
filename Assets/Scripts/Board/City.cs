using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class City : MonoBehaviour
{
    [Header("City Properties")]
    [SerializeField] private string cityName;
    [SerializeField] private int pointWhenConquered;
    [SerializeField] private int multiplierWhenConquered;
    private List<GameObject> territoriesGameObject = new();

    [Header("Territories Quantity")]
    [SerializeField] private int desertQuantity;
    [SerializeField] private int riverQuantity;
    [SerializeField] private int plainsQuantity;
    [SerializeField] private int mountainsQuantity;

    [SerializeField] private Sprite[] territorySprites;

    private int minPoint = 1;
    private int maxPoint = 4;

    void Start()
    {
        List<string> typesAvailable = new() { "DESERT", "RIVER", "PLAINS", "MOUNTAINS" };
        Dictionary<string, Sprite> spritesByName = new()
        {
            { "DESERT", territorySprites[0] },
            { "RIVER", territorySprites[1] },
            { "PLAINS", territorySprites[2] },
            { "MOUNTAINS", territorySprites[3] },
        };

        foreach (Transform child in transform)
        {
            territoriesGameObject.Add(child.gameObject);
        }

        foreach (var territory in territoriesGameObject)
        {
            var territoryPointText = territory.GetComponentInChildren<TextMeshPro>();
            var territorySprite = territory.GetComponent<SpriteRenderer>();
            var territoryInstance = territory.GetComponent<Territory>();

            ChooseTerritoryPoint(territoryPointText, territoryInstance);

            ChooseTerritoryType(typesAvailable, spritesByName, territorySprite, territoryInstance);
        }
    }

    private void ChooseTerritoryPoint(TextMeshPro territoryPointText, Territory territoryInstance)
    {
        int randomPoint = Random.Range(minPoint, maxPoint + 1);
        territoryPointText.text = randomPoint.ToString();
        territoryInstance.Point = randomPoint;
    }

    private void ChooseTerritoryType(List<string> typesAvailable, Dictionary<string, Sprite> spritesByName,
        SpriteRenderer territorySprite, Territory territoryInstance)
    {
        int indexTypeChoosed = Random.Range(0, typesAvailable.Count);
        string typeChoosed = typesAvailable[indexTypeChoosed];

        bool needToRemove = false;
        switch (typeChoosed)
        {
            case "DESERT":
                desertQuantity--;
                needToRemove = desertQuantity == 0;
                break;
            case "RIVER":
                riverQuantity--;
                needToRemove = riverQuantity == 0;
                break;
            case "PLAINS":
                plainsQuantity--;
                needToRemove = plainsQuantity == 0;
                break;
            case "MOUNTAINS":
                mountainsQuantity--;
                needToRemove = mountainsQuantity == 0;
                break;
        }

        if (needToRemove)
        {
            typesAvailable.RemoveAt(indexTypeChoosed);
        }

        territorySprite.sprite = spritesByName[typeChoosed];
        territoryInstance.Type = typeChoosed;
    }
}
