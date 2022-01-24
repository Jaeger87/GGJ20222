using Photon.Pun;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private MatchManager MatchManager = null;

    [SerializeField] private Transform HealthBarFill = null;
    
    [SerializeField]
    private ETeam i_Team;
    
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
    
    private void Awake()
    {
        m_CurrentLife = Life;
        UpdateHealthBar();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_Animator.Play("HitAnimationControlPoint");
        m_AudioSource.PlayOneShot(HitSound);
        if (m_bOffline || PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                //todo: Qui bisogna uccidere il nemico
                m_CurrentLife--;
            
                collision.gameObject.GetComponent<EnemyController>().Die();

                UpdateHealthBar();
                
                if (m_CurrentLife <= 0)
                {
                    MatchManager.GameEnded(i_Team);
                }
                //todo: pensare anche a roba grafica
            }
        }
       
    }

    private void UpdateHealthBar()
    {
        if (m_CurrentLife < 0)
        {
            return;
        }

        if (HealthBarFill == null)
        {
            return;
        }

        Vector3 LocalScale = HealthBarFill.localScale;
        LocalScale.x = Life / m_CurrentLife;
        HealthBarFill.localScale = LocalScale;
    }
}

