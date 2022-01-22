using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MatchManager : MonoBehaviour
{
    private MatchManager Instance;
    private const float SpawnEnemyDeltaTime = 3.0f;
    private float m_fTimeToNextSpawn = SpawnEnemyDeltaTime;
    private PhotonView m_PhotonView;

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
    }

    private void Update()
    {
        m_fTimeToNextSpawn -= Time.deltaTime;
        if (m_fTimeToNextSpawn <= 0)
        {
            //@todo spwna prossimo nemico
        }
    }

    public void GameEnded(ETeam i_Team)
    {
        Debug.Log($"{i_Team} lose");
        ETeam WinnerTeam = i_Team == ETeam.Team1 ? ETeam.Team2 : ETeam.Team1;
        m_PhotonView.RPC("GameOver", RpcTarget.AllBuffered, WinnerTeam);
        
    }
    
    [PunRPC]
    public void GameOver(ETeam i_WinnerTeam)
    {
        //todo: GameOver, decidere cosa mostrare
        
    }
    
}
