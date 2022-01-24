using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MatchManager : MonoBehaviour
{
    private MatchManager Instance;
    private PhotonView m_PhotonView;
    [SerializeField]
    private SpawnManager m_Spawnmanager = null;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        m_PhotonView = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsMasterClient)
        {
            m_PhotonView.RPC("StartGame", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void StartGame()
    {
        m_Spawnmanager.StartGame();
    }

    public void GameEnded(ETeam i_Team)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ETeam WinnerTeam = i_Team == ETeam.Team1 ? ETeam.Team2 : ETeam.Team1;
            m_PhotonView.RPC("GameOver", RpcTarget.AllBuffered, WinnerTeam);
        }
    }
    
    [PunRPC]
    public void GameOver(ETeam i_WinnerTeam)
    {
        Debug.Log($"{i_WinnerTeam} Wins");
        m_Spawnmanager.EndGame();
        //todo: GameOver, decidere cosa mostrare
        
    }
    
}
