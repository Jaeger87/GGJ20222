using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchManager : MonoBehaviourPunCallbacks
{
    private static MatchManager Instance;
    private PhotonView m_PhotonView;
    
    [SerializeField]
    private SpawnManager m_Spawnmanager = null;

    [SerializeField] private GameObject VictoryPanel = null;
    [SerializeField] private Text VictoryLabel;
    
    [SerializeField] private Transform m_ArenaSize;

    public Transform ArenaSize
    {
        get => m_ArenaSize;
    }

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

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public static void LeaveRoom()
    {
        Instance.StartCoroutine(PhotonLeaveRoom());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LeaveRoom();
    }

    private static IEnumerator PhotonLeaveRoom()
    {

        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        
        SceneManager.LoadScene("Lobby");
    }
}
