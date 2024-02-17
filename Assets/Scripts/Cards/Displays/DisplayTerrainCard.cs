using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTerrainCard : DisplayCard<TerrainCard>
{
    public TextMeshProUGUI TerrainTypeText;
    public TextMeshProUGUI StrengthText;

    protected override void Start()
    {
        base.Start();
        StrengthText.text = Card.Strength.ToString();
        TerrainTypeText.text = Card.Type.ToString();   
    }
}
