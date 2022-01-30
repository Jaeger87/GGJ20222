using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerPrefab;
    
    [SerializeField]
    private GameObject EnemyPrefabLeft;

    [SerializeField]
    private GameObject EnemyPrefabRight;
    
    [SerializeField]
    private GameObject ControlPointPrefabTeam1;
    
    [SerializeField]
    private GameObject ControlPointPrefabTeam2;
    
    [SerializeField]
    private Transform Team1SpawnPoint;
    
    [SerializeField]
    private Transform Team2SpawnPoint;

    [SerializeField]
    private Transform ControlPointLeftSpawnPoint;
    
    [SerializeField]
    private Transform ControlPointRightSpawnPoint;
    
    [SerializeField]
    private GameLoading GameLoadingUI;
    
    [FormerlySerializedAs("Enemy1SpawnPoint")] [SerializeField]
    private Transform EnemyLeftSpawnPoint;
    
    [FormerlySerializedAs("Enemy2SpawnPoint")] [SerializeField]
    private Transform EnemyRightSpawnPoint;

    [SerializeField]
    private GameObject WaitingUI = null;

    private const float SpawnEnemyDeltaTime = 20.0f;
    private float m_fTimeToNextSpawn = float.MaxValue;

    private bool m_bGameStarted = false;
    private bool m_bGameEnd = false;

    private void Start()
    {
        if (WaitingUI != null)
        {
            WaitingUI.SetActive(PhotonNetwork.IsMasterClient);   
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

    public void SpawnControlPoints()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject ControlPointLeft = PhotonNetwork.Instantiate(ControlPointPrefabTeam1.name,
                ControlPointLeftSpawnPoint.position,
                Quaternion.identity);

            GameObject ControlPointRight = PhotonNetwork.Instantiate(ControlPointPrefabTeam2.name,
                ControlPointRightSpawnPoint.position,
                Quaternion.identity);
        }
    }
    
    public void StartGame()
    {
        SpawnControlPoints();
        if (WaitingUI != null)
        {
            WaitingUI.SetActive(false);
        }
        if (GameLoadingUI != null)
        {
            GameLoadingUI.gameObject.SetActive(true);
            GameLoadingUI.StartCountdown();
            GameLoadingUI.CountdownEnded += AfterStartGame;
        }
        else
        {
            AfterStartGame();
        }
    }

    private void AfterStartGame()
    {
        SpawnPlayers();
        SpawnEnemy();
        m_bGameStarted = true;
        m_fTimeToNextSpawn = SpawnEnemyDeltaTime;

        if (GameLoadingUI != null)
        {
            GameLoadingUI.CountdownEnded -= AfterStartGame;
            GameLoadingUI.gameObject.SetActive(false);
        }
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
            GameObject EnemyForPlayer1 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefabLeft.name,
                EnemyLeftSpawnPoint.position, Quaternion.identity);

            GameObject EnemyForPlayer2 = PhotonNetwork.Instantiate("Enemy/" + EnemyPrefabRight.name,
                EnemyRightSpawnPoint.position, Quaternion.identity);

            /* Il codice da qui non è sincronizzato tra i client
            EnemyController EnemyController_p1 = EnemyForPlayer1.GetComponent<EnemyController>();
            EnemyController EnemyController_p2 = EnemyForPlayer2.GetComponent<EnemyController>();

            
            EnemyController_p1.SetLookingRight(true);
            EnemyController_p2.SetLookingRight(false);
            EnemyController_p1.SetTeam(ETeam.Team1);
            EnemyController_p2.SetTeam(ETeam.Team2);
            */
        }
    }
}
