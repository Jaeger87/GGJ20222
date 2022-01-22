using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Graphics; 
    
    [SerializeField]
    private Sprite Team1Graphics;
    
    [SerializeField]
    private Sprite Team2Graphics;
    
    private ETeam m_Team;
    private PhotonView m_PhotonView;
    
    public void SetTeam(ETeam i_Team)
    {
        if (m_PhotonView.IsMine)
        {
            m_PhotonView.RPC("SetupPlayer", RpcTarget.AllBuffered, i_Team);
        }
    }
    
    [PunRPC]
    private void SetupPlayer(ETeam i_Team)
    {
        m_Team = i_Team;
        if (Graphics != null)
        {
            Graphics.sprite = m_Team == ETeam.Team1 ? Team1Graphics : Team2Graphics;
        }
    }

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        if (m_PhotonView != null && !m_PhotonView.IsMine)
        {
            GetComponent<PlayerMovement>().enabled = false;
        }
    }

    public void Flip()
    {
        Vector3 scale = Graphics.transform.localScale;
        scale.x *= -1f;
        Graphics.transform.localScale = scale;
    }
}
