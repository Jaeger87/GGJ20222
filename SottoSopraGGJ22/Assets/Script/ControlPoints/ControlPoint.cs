using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private MatchManager MatchManager = null;
    [SerializeField] private ETeam i_Team;
    [SerializeField] private int life = 0;
    
    [SerializeField]
    private Animator m_Animator;
    
    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip HitSound;
    
    private void Start()
    {
        MatchManager = FindObjectOfType<MatchManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_Animator.Play("HitAnimationControlPoint");
        m_AudioSource.PlayOneShot(HitSound);
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                //todo: Qui bisogna uccidere il nemico
                life--;
            
                collision.gameObject.GetComponent<EnemyController>().Die();
                if (life <= 0)
                {
                    MatchManager.GameEnded(i_Team);
                }
                //todo: pensare anche a roba grafica
            }
        }
       
    }
}

