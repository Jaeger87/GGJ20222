using System;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class UIElement_RoomItem: MonoBehaviour
    {
        [SerializeField] private Text m_Label;

        public event Action<string> onRoomClick;

        private RoomInfo m_RoomInfo = null;
        
        
        public void Setup(RoomInfo roomItem)
        {
            m_RoomInfo = roomItem;
            m_Label.text = m_RoomInfo.Name;
        }

        public void JoinRoom()
        {
            onRoomClick?.Invoke(m_RoomInfo.Name);
        }
    }
}