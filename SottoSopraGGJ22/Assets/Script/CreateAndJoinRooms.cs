using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject LoadingUI = null;
    
    [SerializeField]
    private InputField RoomName;
    
    [SerializeField]
    private InputField PlayerName;

    private void Awake()
    {
        string Name = PlayerPrefs.GetString("player_name");
        if (!String.IsNullOrEmpty(Name))
        {
            if (PlayerName != null)
            {
                PlayerName.text = Name;
            }
        }
    }

    public void CreateRoom()
    {
        if (RoomName.text == "")
        {
            return;
        }

        SaveName();
        
        PhotonNetwork.JoinOrCreateRoom(RoomName.text, GetRoomConfig(), TypedLobby.Default);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }
    }

    private void SaveName()
    {
        if (PlayerName != null)
        {
            string Name = PlayerName.text;
            PlayerPrefs.SetString("player_name", String.IsNullOrEmpty(Name) ? "EmptyNameBruh" : Name);
        }
    }

    public void JoinRoom()
    {
        if (RoomName.text == "")
        {
            return;
        }

        SaveName();
        
        PhotonNetwork.JoinOrCreateRoom(RoomName.text, GetRoomConfig(), TypedLobby.Default);
        
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
