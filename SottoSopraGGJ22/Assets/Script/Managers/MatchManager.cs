using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    private static MatchManager Instance;
    private PhotonView m_PhotonView;
    
    [SerializeField]
    private SpawnManager m_Spawnmanager = null;

    [SerializeField] private GameObject VictoryPanel = null;
    [SerializeField] private Text VictoryLabel;

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

    public static MatchManager GetMatchManager()
    {
        if (Instance != null)
            return Instance;
        return null;
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
        m_Spawnmanager.EndGame();
        //todo: GameOver, decidere cosa mostrare

        if (VictoryPanel != null)
        {
            VictoryPanel.SetActive(true);

            if (VictoryLabel != null)
            {
                string playerName = i_WinnerTeam == ETeam.Team1 ? "Player 2" : "Player 1";
                VictoryLabel.text = $"{playerName} Wins";
            }
        }

    }
    
}
