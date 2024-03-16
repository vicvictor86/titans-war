using Domain;
using System;
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

    [Header("Territories Quantity ReadOnly")]
    [SerializeField] private int desertQuantityReadOnly;
    [SerializeField] private int riverQuantityReadOnly;
    [SerializeField] private int plainsQuantityReadOnly;
    [SerializeField] private int mountainsQuantityReadOnly;
    [SerializeField] private int totalTerrainTypesQuantityReadOnly;

    [Header("Prefabs")]
    [SerializeField] public GameObject cityInfoPrefab;
    [SerializeField] public GameObject attackButton;

    [SerializeField] private int totalTerrainTypesQuantity => desertQuantity + riverQuantity + plainsQuantity + mountainsQuantity;

    [SerializeField] private List<Sprite> territorySprites;

    private int minPoint = 1;
    private int maxPoint = 4;

    [Serializable]
    private struct CityBenefits 
    {
        public int MultiplicativeCoefficient;
        public int AdditiveCoefficient;
        public string Description => $"{MultiplicativeCoefficient}X + {AdditiveCoefficient}";
    };

    [Header("Benefits")]
    [SerializeField] private CityBenefits cityBenefits;

    public PlayerDeck Owner { get; private set; }

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
        List<TerrainType> typesAvailable = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();
        Dictionary<TerrainType, Sprite> spritesByName = new()
        {
            { TerrainType.DESERT, territorySprites[0] },
            { TerrainType.MOUNTAINS, territorySprites[1] },
            { TerrainType.PLAINS, territorySprites[2] },
            { TerrainType.RIVER, territorySprites[3] },
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

            territoryInstance.SetCity(this);

            ChooseTerritoryPoint(territoryPointText, territoryInstance);

            ChooseTerritoryType(typesAvailable, spritesByName, territorySprite, territoryInstance);

            allCitiesEdges.AddRange(CityBorder.CalculateEdges(territorySprite));
        }

        var territoryLineRenderer = gameObject.GetComponent<LineRenderer>();

        List<Vector3> cityBorderVertexes = new();
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
                if (!cityBorderVertexes.Any(edge => Vector3.Distance(edge, cityEdge) < 0.1f))
                {
                    cityBorderVertexes.Add(cityEdge);
                }
            }
        }

        CityBorder.CreateCityBorder(cityBorderVertexes, territoryLineRenderer);
    }

    public int GetDesertQuantity()
    {
        return desertQuantityReadOnly;
    }

    public int GetPlainsQuantity()
    {
        return plainsQuantityReadOnly;
    }

    public int GetRiverQuantity()
    { 
        return riverQuantityReadOnly; 
    }

    public int GetMountainsQuantity()
    {
        return mountainsQuantityReadOnly;
    }

    public string GetBenefitDescription()
    {
        return cityBenefits.Description;
    }

    public void ValidAndSetOwnership(PlayerDeck player)
    {
        int playerTerritoryCount = territoriesGameObject.Select(territory => territory.GetComponent<Territory>())
            .Where(territory => territory.Owner == player).Count();
        if (playerTerritoryCount >= 7)
        {
            Owner = player;
        }
    }

    public int GetPointsForPlayer(PlayerDeck player)
    {
        var basePoints = territoriesGameObject.Select(territory => territory.GetComponent<Territory>())
            .Where(territory => territory.Owner == player).Sum(territoty => territoty.Point);

        return player == Owner ? 
            cityBenefits.MultiplicativeCoefficient * basePoints + cityBenefits.AdditiveCoefficient
            : basePoints;
    }

    private void ChooseTerritoryPoint(TextMeshPro territoryPointText, Territory territoryInstance)
    {
        int randomPoint = UnityEngine.Random.Range(minPoint, maxPoint + 1);
        territoryPointText.text = randomPoint.ToString();
        territoryInstance.Point = randomPoint;
    }

    private void ChooseTerritoryType(List<TerrainType> typesAvailable, Dictionary<TerrainType, Sprite> spritesByName,
        SpriteRenderer territorySprite, Territory territoryInstance)
    {
        int indexTypeChoosed = UnityEngine.Random.Range(0, typesAvailable.Count);
        TerrainType typeChoosed = typesAvailable[indexTypeChoosed];

        bool needToRemove = false;
        switch (typeChoosed)
        {
            case TerrainType.DESERT:
                desertQuantity--;
                needToRemove = desertQuantity == 0;
                break;
            case TerrainType.RIVER:
                riverQuantity--;
                needToRemove = riverQuantity == 0;
                break;
            case TerrainType.PLAINS:
                plainsQuantity--;
                needToRemove = plainsQuantity == 0;
                break;
            case TerrainType.MOUNTAINS:
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
