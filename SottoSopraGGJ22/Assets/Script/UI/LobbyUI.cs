using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Script;
using UnityEngine;

public class LobbyUI :  MonoBehaviourPunCallbacks
{
    [SerializeField] private UIElement_RoomItem RoomItemPrefab;
    
    [SerializeField] private Transform m_RoomListContent;

    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     foreach (Transform child in m_RoomListContent)
    //     {
    //         Destroy(child);
    //     }
    //     foreach (var roomItem in roomList)
    //     {
    //         UIElement_RoomItem go = Instantiate(RoomItemPrefab, Vector3.zero, Quaternion.identity, m_RoomListContent);
    //         go.Setup(roomItem);
    //     }
    // }
}
