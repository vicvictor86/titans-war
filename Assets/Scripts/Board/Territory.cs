using Domain;
using UnityEngine;
using UnityEngine.EventSystems;

public class Territory : MonoBehaviour
{
    public TerrainType Type;
    public int Point;
    private PlayerDeck owner;

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var city = transform.parent.GetComponent<City>();
            var cityInfoPrefab = city.cityInfoPrefab;
            GameManager.instance.InstantiateCityInfo(city, Type, this);
        }
    }

    public void SetOwner(PlayerDeck newOwner)
    {
        owner = newOwner;
    }


    
}
