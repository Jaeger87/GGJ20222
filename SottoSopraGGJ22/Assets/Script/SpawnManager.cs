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

    private const float SpawnEnemyDeltaTime = 5.0f;
    private float m_fTimeToNextSpawn = float.MaxValue;

    private bool m_bGameStarted = false;
    private bool m_bGameEnd = false;
    
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // team 1
        }
        else
        {
            // team 2
            
        }

        
    }

    public void SpawnPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject Player1 = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team1SpawnPoint.position,
                Quaternion.identity);
            Player1.GetComponent<PlayerController>().SetTeam(ETeam.Team1);
        }

        else
        {
            GameObject Player2 = PhotonNetwork.Instantiate("Player/" + PlayerPrefab.name, Team2SpawnPoint.position,
                Quaternion.identity);
            Player2.GetComponent<PlayerController>().SetTeam(ETeam.Team2);
        }
    }
    
    public void StartGame()
    {
        SpawnPlayers();
        SpawnEnemy();
        m_bGameStarted = true;
        m_fTimeToNextSpawn = SpawnEnemyDeltaTime;
    }

    public void EndGame()
    {
        m_bGameEnd = true;
    }

    private void Update()
    {
        if (m_bGameStarted && !m_bGameEnd)
        {
            m_fTimeToNextSpawn -= Time.deltaTime;
            if (m_fTimeToNextSpawn <= 0)
            {
                SpawnEnemy();
                m_fTimeToNextSpawn = SpawnEnemyDeltaTime;
            }
        }
    }

    public void SpawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject EnemyForPlayer1 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefab.name, Enemy1SpawnPoint.position, Quaternion.identity);
        
            GameObject EnemyForPlayer2 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefab.name, Enemy2SpawnPoint.position, Quaternion.identity);
            
            //Qui settare cose sui nemici
        }
        

            
    }
}
