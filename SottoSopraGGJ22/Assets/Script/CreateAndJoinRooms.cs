using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject RandomFailedLog = null;
    
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
        
        PhotonNetwork.JoinOrCreateRoom(RoomName.text.ToLower(), GetRoomConfig(), TypedLobby.Default);
        
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
        
        if (RandomFailedLog != null)
        {
            RandomFailedLog.SetActive(true);
        }

        SaveName();
        
        PhotonNetwork.JoinOrCreateRoom(RoomName.text.ToLower(), GetRoomConfig(), TypedLobby.Default);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }
    }
    
    public void JoinRoom(string i_RoomID)
    {
        if (RandomFailedLog != null)
        {
            RandomFailedLog.SetActive(true);
        }
        
        SaveName();
        
        PhotonNetwork.JoinOrCreateRoom(i_RoomID.ToLower(), GetRoomConfig(), TypedLobby.Default);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }
    }
    
    public void JoinRandomRoom()
    {
        SaveName();

        PhotonNetwork.JoinRandomRoom();

        if (LoadingUI != null)
        {
            LoadingUI.SetActive(true);
        }

        if (RandomFailedLog != null)
        {
            RandomFailedLog.SetActive(false);
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
        if (RandomFailedLog != null)
        {
            RandomFailedLog.SetActive(false);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(false);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        
        if (LoadingUI != null)
        {
            LoadingUI.SetActive(false);
        }
        
        if (RandomFailedLog != null)
        {
            RandomFailedLog.SetActive(true);
        }
    }
}
