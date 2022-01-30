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

    private bool m_bIsGameStarted = false;

    public static bool IsGameStarted => Instance.m_bIsGameStarted;
    
    public static Transform ArenaSize
    {
        get => Instance.m_ArenaSize;
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
            m_PhotonView.RPC("GameOver", RpcTarget.AllBuffered, i_Team);
        }
    }
    
    [PunRPC]
    public void GameOver(ETeam i_LoserTeam)
    {
        m_Spawnmanager.EndGame();
        //todo: GameOver, decidere cosa mostrare

        if (VictoryPanel != null)
        {
            VictoryPanel.SetActive(true);

            if (VictoryLabel != null)
            {
                string playerName = i_LoserTeam == ETeam.Team1 ? "Doors" : "Pears";
                VictoryLabel.text = $"{playerName} get hacked!";
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

    public static void GameStarted()
    {
        Instance.m_bIsGameStarted = true;
    }
}
