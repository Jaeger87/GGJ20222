using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ETeam m_Team;
    private PhotonView m_PhotonView;
    
    public void SetTeam(ETeam i_Team)
    {
        m_Team = i_Team;
    }

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        if (m_PhotonView != null && !m_PhotonView.IsMine)
        {
            GetComponent<PlayerMovement>().enabled = false;
        }
    }
}
