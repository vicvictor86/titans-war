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
            Destroy(GameObject.FindWithTag("CityInfo"));
            Destroy(GameObject.FindWithTag("AttackButton"));
            var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
            var city = transform.parent.GetComponent<City>();
            var cityInfoPrefab = city.cityInfoPrefab;
            var infoInstance = Instantiate(cityInfoPrefab, new Vector3(200, 200, 0), Quaternion.identity, canvas.transform);
            infoInstance.GetComponent<DisplayCityInfo>().City = city;
            infoInstance.transform.SetAsFirstSibling();

            if (GameManager.instance.actualPlayer.ListTerrainTypesDisponibleToAttack().Contains(Type))
            {
                var attackButton = Instantiate(city.attackButton, new Vector3(200, 100, 0), Quaternion.identity, canvas.transform);
                attackButton.GetComponent<AttackButton>().territory = this;
            };
        }
    }

    public void SetOwner(PlayerDeck newOwner)
    {
        owner = newOwner;
    }


    
}
