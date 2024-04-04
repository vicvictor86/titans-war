using Domain;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Territory : MonoBehaviour, IPointerClickHandler
{
    public TerrainType Type;
    public int Point;
    public PlayerDeck Owner { get; private set; }
    public City City { get; private set; }

    [Header("Prefabs")]
    public GameObject territoryInfoPrefab;

    public void SetOwner(PlayerDeck newOwner)
    {
        Owner = newOwner;
        var highLightTerritory = GetComponent<HighlightTerritory>();
        if (Owner.PlayerSide == "Spartha")
        {
            highLightTerritory.SetColorAndPreviousColor(new Color(1, 0, 0, 0.5f));
        }
        else
        {
            highLightTerritory.SetColorAndPreviousColor(new Color(0, 0, 1, 0.5f));
        }
        
        highLightTerritory.wasClicked = false;
    }

    public void SetCity(City city)
    {
        City = city;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var city = transform.parent.GetComponent<City>();
            var cityInfoPrefab = city.cityInfoPrefab;
            GameManager.instance.InstantiateCityAndTerritoryInfo(city, Type, this);
        }
    }
}
