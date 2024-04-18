using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public GameObject connectedScreen;
    public GameObject loginScreen;
    public GameObject disconnectedScreen;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void HandleLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            AuthenticationValues authValues = new AuthenticationValues();
            
            authValues.AuthType = CustomAuthenticationType.Custom;
            authValues.AddAuthParameter("user", username);
            authValues.AddAuthParameter("pass", password);
            authValues.UserId = username; 

            PhotonNetwork.AuthValues = authValues;

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Por favor, preencha os campos de nome de usuário e senha.");
        }
    }

    public void HandleCreateAccount()
    {
        Application.OpenURL("https://titan-wars-server.onrender.com/");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor do Photon.");
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        loginScreen.SetActive(false);
        connectedScreen.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnectedScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        connectedScreen.SetActive(true);
    }
}
