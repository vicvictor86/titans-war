using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class City : MonoBehaviour
{
    [Header("City Properties")]
    [SerializeField] private string cityName;
    [SerializeField] private int pointWhenConquered;
    [SerializeField] private int multiplierWhenConquered;
    [SerializeField] private Tilemap tilemap;
    private List<GameObject> territoriesGameObject = new();

    [Header("Territories Quantity")]
    [SerializeField] private int desertQuantity;
    [SerializeField] private int riverQuantity;
    [SerializeField] private int plainsQuantity;
    [SerializeField] private int mountainsQuantity;

    [Header("Desert Quantity ReadOnly")]
    [SerializeField] private int desertQuantityReadOnly;
    [SerializeField] private int riverQuantityReadOnly;
    [SerializeField] private int plainsQuantityReadOnly;
    [SerializeField] private int mountainsQuantityReadOnly;
    [SerializeField] private int totalTerrainTypesQuantityReadOnly;

    [SerializeField] private int totalTerrainTypesQuantity => desertQuantity + riverQuantity + plainsQuantity + mountainsQuantity;

    [SerializeField] private List<Sprite> territorySprites;

    private int minPoint = 1;
    private int maxPoint = 4;

    void Start()
    {
        desertQuantityReadOnly = desertQuantity;
        riverQuantityReadOnly = riverQuantity;
        plainsQuantityReadOnly = plainsQuantity;
        mountainsQuantityReadOnly = mountainsQuantity;
        totalTerrainTypesQuantityReadOnly = totalTerrainTypesQuantity;

        if (totalTerrainTypesQuantity < 12)
        {
            Debug.LogError($"City {cityName} doesn't have 12 territories, instead have {totalTerrainTypesQuantity}." +
                $"Please go to 'Territories Quantity' and define the quantity of each territory.");
        }

        territorySprites = Resources.LoadAll<Sprite>("Sprites/HexTerrains").ToList();
        List<string> typesAvailable = new() { "DESERT", "MOUNTAINS", "PLAINS", "RIVER" };
        Dictionary<string, Sprite> spritesByName = new()
        {
            { "DESERT", territorySprites[0] },
            { "MOUNTAINS", territorySprites[1] },
            { "PLAINS", territorySprites[2] },
            { "RIVER", territorySprites[3] },
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
            var territoryLineRenderer = territory.GetComponent<LineRenderer>();

            ChooseTerritoryPoint(territoryPointText, territoryInstance);

            ChooseTerritoryType(typesAvailable, spritesByName, territorySprite, territoryInstance);

            CityBorder.CreateCityBorder(territorySprite, territoryLineRenderer);
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
