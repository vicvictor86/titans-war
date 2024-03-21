﻿using TMPro;
using UnityEngine;

public class DisplayTerritoryInfo : MonoBehaviour
{
    public Territory Territory;
    public TextMeshProUGUI Player;

    protected void Start()
    {
        //Deve ser trocado para nome do jogador ou coisa do tipo
        Player.text = (GameManager.instance.playerList.IndexOf(Territory.Owner) + 1).ToString();
    }

}