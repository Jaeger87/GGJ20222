using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Script;
using UnityEngine;
using UnityEngine.Events;

public class LobbyUI :  MonoBehaviourPunCallbacks
{
    [SerializeField] private UIElement_RoomItem RoomItemPrefab;
    [SerializeField] private Transform m_RoomListContent;
    [SerializeField] private GameObject m_CreditsPage = null; 
    
    
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public UnityEvent<string> onRoomClick = new UnityEvent<string>();

    public void OpenCredits()
    {
        m_CreditsPage.SetActive(true);
    }
    
    public void CloseCredits()
    {
        m_CreditsPage.SetActive(false);
    }
    
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for(int i=0; i<roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }

        RenderRoomList();
    }

    private void RenderRoomList()
    {
        foreach (Transform child in m_RoomListContent)
        {
            UIElement_RoomItem roomItem = child.GetComponent<UIElement_RoomItem>();
            
            if (roomItem) 
               roomItem.onRoomClick -= OnRoomClick;
           
            Destroy(child.gameObject);
        }
        
        foreach (var roomInfoMap in cachedRoomList)
        {
            UIElement_RoomItem item = Instantiate(RoomItemPrefab, m_RoomListContent);
            
            item.Setup(roomInfoMap.Value);

            item.onRoomClick += OnRoomClick;
        }
    }

    private void OnRoomClick(string i_RoomID)
    {
        onRoomClick?.Invoke(i_RoomID);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
}
