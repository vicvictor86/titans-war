using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Login : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void HandleLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = username;
            PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Por favor, preencha os campos de nome de usuário e senha.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor do Photon.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Jogador autenticado e conectado ao lobby.");
    }
}