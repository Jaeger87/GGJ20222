using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Graphics;

    [SerializeField]
    private PlayerHitHintController HitHintController;
    
    [SerializeField]
    private UIPage_PlayerGame PlayerInGameUI;
    
    [SerializeField]
    private Sprite Team1Graphics;
    
    [SerializeField]
    private Sprite Team2Graphics;
    
    private ETeam m_Team;
    private PhotonView m_PhotonView;

    private PlayerMovement m_PlayerMovement;
    
    public void SetTeam(ETeam i_Team)
    {
        if (m_PhotonView.IsMine)
        {
            m_PhotonView.RPC("SetupPlayer", RpcTarget.AllBuffered, i_Team);
        }
    }

    
    public void SendJump()
    {
        if (m_PhotonView.IsMine)
        {
            m_PhotonView.RPC("AddJumpForce", RpcTarget.AllBuffered);
        }
    }
    public void SendDash()
    {
        if (m_PhotonView.IsMine)
        {
            m_PhotonView.RPC("AddDashForce", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void AddJumpForce()
    {
        if (!m_PhotonView.IsMine)
        {
            m_PlayerMovement.AddJumpToRigidBody();
        }
    }
    
    [PunRPC]
    public void AddDashForce()
    {
        if (!m_PhotonView.IsMine)
        {
            m_PlayerMovement.AddDashToRigidBody();
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

        if (PlayerInGameUI != null)
        {
            PlayerInGameUI.SetNameLabelByTeam(i_Team);
        }
    }

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        m_PlayerMovement = GetComponent<PlayerMovement>();

        if (m_PhotonView != null && !m_PhotonView.IsMine)
        {
            m_PlayerMovement.SetAsNetworkPlayer();
            SetDashHintActive(false);
        }
    }

    public void Flip()
    {
        Vector3 scale = Graphics.transform.localScale;
        scale.x *= -1f;
        Graphics.transform.localScale = scale;
    }

    public void SetDashHintActive(bool i_bActive)
    {
        HitHintController.gameObject.SetActive(i_bActive);
    }
}
