using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private LoadingUI m_LoadingUI = null;
    
    
    private TypedLobby customLobby = new TypedLobby("hackYouLobby", LobbyType.Default);
    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        
        m_LoadingUI?.SetLoadingText("Connecting to server...");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(customLobby);
        m_LoadingUI?.SetLoadingText("Joining Lobby...");
    }

    public override void OnJoinedLobby()
    {
        m_LoadingUI?.SetLoadingText("Loading done...");
        Invoke(nameof(LoadLobbyScene), 1f);
    }

    private void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
