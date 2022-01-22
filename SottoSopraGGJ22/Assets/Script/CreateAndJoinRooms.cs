using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject LoadingUI = null;
    
    public InputField createInput;
    public InputField joinInput;


    public void CreateRoom()
    {
        if (createInput.text == "")
        {
            return;
        }
        PhotonNetwork.JoinOrCreateRoom(joinInput.text, GetRoomConfig(), TypedLobby.Default);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }
    }
    
    public void JoinRoom()
    {
        if (joinInput.text == "")
        {
            return;
        }

        PhotonNetwork.JoinOrCreateRoom(joinInput.text, GetRoomConfig(), TypedLobby.Default);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }
    }

    private RoomOptions GetRoomConfig()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(false);
        }
    }
}
