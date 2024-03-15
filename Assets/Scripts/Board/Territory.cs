using Domain;
using UnityEngine;
using UnityEngine.EventSystems;

public class Territory : MonoBehaviour, IPointerClickHandler
{
    public TerrainType Type;
    public int Point;
    private PlayerDeck owner;

    public void SetOwner(PlayerDeck newOwner)
    {
        owner = newOwner;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var city = transform.parent.GetComponent<City>();
            var cityInfoPrefab = city.cityInfoPrefab;
            GameManager.instance.InstantiateCityInfo(city, Type, this);
        }
    }
}
