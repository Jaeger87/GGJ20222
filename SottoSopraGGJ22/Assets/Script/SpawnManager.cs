using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerPrefab;
    
    [SerializeField]
    private GameObject EnemyPrefab;

    [SerializeField]
    private Transform Team1SpawnPoint;
    
    [SerializeField]
    private Transform Team2SpawnPoint;
    
    [SerializeField]
    private Transform Enemy1SpawnPoint;
    
    [SerializeField]
    private Transform Enemy2SpawnPoint;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // team 1
            GameObject Player = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team1SpawnPoint.position, Quaternion.identity);
            Player.GetComponent<PlayerController>().SetTeam(ETeam.Team1);
            SpawnEnemy();
        }
        else
        {
            // team 2
            GameObject Player = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team2SpawnPoint.position, Quaternion.identity);
            Player.GetComponent<PlayerController>().SetTeam(ETeam.Team2);
        }

        
    }

    public void SpawnEnemy()
    {
        GameObject EnemyForPlayer1 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefab.name, Enemy1SpawnPoint.position, Quaternion.identity);
        GameObject EnemyForPlayer2 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefab.name, Enemy2SpawnPoint.position, Quaternion.identity);
        
        //Qui settare cose sui nemici
    }
}
