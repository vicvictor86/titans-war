using TMPro;
using UnityEngine;

public class DisplayPoints : MonoBehaviour
{
    public PlayerDeck Player;
    public TextMeshProUGUI Points;

    protected void Start()
    {
        Points.text = Player.GetPoints().ToString();
    }

    protected void Update()
    {
        Points.text = Player.GetPoints().ToString();
    }
}
