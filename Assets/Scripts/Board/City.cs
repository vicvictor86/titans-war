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

    public GameObject TESTE;

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

        List<Vector3> allCitiesEdges = new();
        foreach (var territory in territoriesGameObject)
        {
            var territoryPointText = territory.GetComponentInChildren<TextMeshPro>();
            var territorySprite = territory.GetComponent<SpriteRenderer>();
            var territoryInstance = territory.GetComponent<Territory>();

            ChooseTerritoryPoint(territoryPointText, territoryInstance);

            ChooseTerritoryType(typesAvailable, spritesByName, territorySprite, territoryInstance);

            allCitiesEdges.AddRange(CityBorder.CalculateEdges(territorySprite));
        }

        var territoryLineRenderer = gameObject.GetComponent<LineRenderer>();

        foreach (var teste in allCitiesEdges)
        {
            Debug.Log($"X: {Mathf.Round(teste.x * 10f) / 10f} Y: {Mathf.Round(teste.y * 10f) / 10f}");
        }

        List<Vector3> distinctList = new() { allCitiesEdges.FirstOrDefault() };
        
        foreach (var cityEdge in allCitiesEdges)
        {
            int matchQuantity = 0;
            foreach(var cityToCompare in allCitiesEdges)
            {
                if (cityEdge == cityToCompare)
                {
                    continue;
                }

                var distance = Vector3.Distance(cityEdge, cityToCompare);       
                
                if (distance < 0.1f)
                {
                    matchQuantity++;
                }
            }

            if (matchQuantity < 2)
            {
                distinctList.Add(cityEdge);
            }
        }

        //List<Vector3> citiesBorderEdges = allCitiesEdges
        //    .GroupBy(edge => new { X = Mathf.Round(edge.x * 10f) / 10f, Y = Mathf.Round(edge.y * 10f) / 10f })
        //    .Where(group => group.Count() == 1)
        //    .Select(group => group.FirstOrDefault())
        //    .ToList();


        foreach (var citieEdges in distinctList)
        {
            Instantiate(TESTE, citieEdges, Quaternion.identity);
        }

        //CityBorder.CreateCityBorder(citiesBorderEdges, territoryLineRenderer);
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
