using System;
using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerPrefab;

    [SerializeField]
    private Transform Team1SpawnPoint;
    
    [SerializeField]
    private Transform Team2SpawnPoint;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // team 1
            GameObject Player = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team1SpawnPoint.position, Quaternion.identity);
            Player.GetComponent<PlayerController>().SetTeam(ETeam.Team1);
        }
        else
        {
            // team 2
            GameObject Player = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team2SpawnPoint.position, Quaternion.identity);
            Player.GetComponent<PlayerController>().SetTeam(ETeam.Team2);
        }
    }
}
