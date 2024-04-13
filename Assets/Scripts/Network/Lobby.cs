using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    public void HandleCreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoom.text, new RoomOptions { MaxPlayers = 2,  }, null);
    }

    public void HandleJoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined success");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room failed: " + returnCode + " Message: " + message);
    }
}
