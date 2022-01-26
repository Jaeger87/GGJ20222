using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class UIElement_RoomItem: MonoBehaviour
    {
        [SerializeField] private Text m_Label; 
        public void Setup(RoomInfo roomItem)
        {
            m_Label.text = roomItem.Name;
        }
    }
}