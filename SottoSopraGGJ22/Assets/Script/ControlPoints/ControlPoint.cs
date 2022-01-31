using Photon.Pun;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private MatchManager m_MatchManager = null;

    private PhotonView m_PhotonView;
    
    [SerializeField] private Transform HealthBarFill = null;
    
    [SerializeField]
    private ETeam m_Team;
    
    [SerializeField] 
    private int Life = 0;

    private int m_CurrentLife = 0;
    
    [SerializeField]
    private Animator m_Animator;
    
    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip HitSound;

    private bool m_bOffline => !PhotonNetwork.IsConnected;

    public void SetTeam(ETeam i_Team)
    {
        m_Team = i_Team;
    }
    
    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        m_CurrentLife = Life;
        m_MatchManager = MatchManager.GetMatchManager();
        
        UpdateHealthBar();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_Animator.Play("HitAnimationControlPoint");
        m_AudioSource.PlayOneShot(HitSound);
        if (m_bOffline || PhotonNetwork.IsMasterClient)
            if (collision.gameObject.CompareTag("Enemy")) {
                collision.gameObject.GetComponent<EnemyController>().Die();
                if (m_bOffline)
                    Damage();
                else
                    m_PhotonView.RPC("Damage", RpcTarget.AllBuffered);
            }
    }
    
    [PunRPC]
    private void Damage()
    {
        m_CurrentLife--;
        UpdateHealthBar();
        if (m_CurrentLife <= 0)
        {
            m_MatchManager.GameEnded(m_Team);
        }
    }
    
    private void UpdateHealthBar()
    {
        if (m_CurrentLife < 0)
            return;

        if (HealthBarFill == null)
        {
            return;
        }

        Vector3 LocalScale = HealthBarFill.localScale;
        LocalScale.x = 1 - (float)m_CurrentLife / Life;
        HealthBarFill.localScale = LocalScale;
    }
}

