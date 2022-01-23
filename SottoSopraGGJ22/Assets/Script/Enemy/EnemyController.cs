using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EnemyController : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private float MovementSpeed = 5f;

    [SerializeField]
    private Transform FrontWallCheck = null;
    
    [SerializeField]
    private LayerMask FloorMask;

    [SerializeField]
    private Animator m_Animator = null;

    private ETeam m_TargetTeam = ETeam.Team1;

    private Rigidbody2D m_Rigidbody;
    
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.3f);

    private bool m_bIsCollidingForward = false;

    private bool m_bLookingRight = true;
    private bool m_bIsMovingOnStairs = false;
    private bool m_bIsDead = false;

    private bool m_bFloorCheckEnabled = true; 

    private Vector3 m_ObjectivePosition = Vector3.zero;
    
    private Vector3 m_TargetStairDirection;
    private EStairPoint m_StairPointClimbStart = EStairPoint.None;
    
    private PhotonView m_PhotonView;

    private bool m_bOffline => !PhotonNetwork.IsConnected;

    private bool m_bvaluesReceived = false;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        if (!PhotonNetwork.IsMasterClient)
        {
            m_Rigidbody.isKinematic = true;
        }
        m_PhotonView = GetComponent<PhotonView>();
        SearchObjective();
    }

    private void SearchObjective()
    {
        GameObject Objective = GameObject.FindWithTag($"ControlPoint_{m_TargetTeam}");

        if (Objective != null)
        {
            m_ObjectivePosition = Objective.transform.position;
        }
    }

    private void Update()
    {
        if (m_bOffline || PhotonNetwork.IsMasterClient)
        {
            CheckCollisions();
            if (m_bIsCollidingForward)
            {
                Flip();
            }
        }
    }

    private void LateUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            m_Animator.SetBool("Climb", !m_bIsDead && m_bIsMovingOnStairs);
            m_Animator.SetBool("Die", m_bIsDead);
        }
    }

    private void FixedUpdate()
    {
        if (m_bIsDead)
        {
            return;
        }
        
        if (m_bOffline || PhotonNetwork.IsMasterClient)
        {
            if (m_Rigidbody == null)
            {
                return;
            }

            m_Rigidbody.isKinematic = m_bIsMovingOnStairs;

            if (m_bIsMovingOnStairs)
            {
                m_Rigidbody.MovePosition(transform.position + m_TargetStairDirection * Time.fixedDeltaTime * 2f);
                return;
            }

            if (!m_bvaluesReceived)
            {
                m_Rigidbody.velocity = new Vector2(
                    (m_bLookingRight ? transform.right : -transform.right).x * MovementSpeed,
                    m_Rigidbody.velocity.y);
            }
        }

        m_bvaluesReceived = false;
    }

    private void Flip()
    {
        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;
        transform.localScale = LocalScale;
        
        m_bLookingRight = !m_bLookingRight;
    }

    private void CheckCollisions()
    {
        if (m_bIsDead)
        {
            return;
        }
        
        if (!m_bFloorCheckEnabled)
        {
            return;
        }
        
        if (FrontWallCheck != null)
        {
            if (Physics2D.OverlapBox(FrontWallCheck.position, m_FrontWallCheckSize, 0, FloorMask))
            {
                m_bIsCollidingForward = true;
            }
            else
            {
                m_bIsCollidingForward = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(FrontWallCheck.position, m_FrontWallCheckSize);
    }

    public void OnStairsEnter(Vector3 i_StartPoint, Vector3 i_EndPoint, EStairPoint i_eStairPoint)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (i_eStairPoint == m_StairPointClimbStart)
            {
                 return;
            }
        
            if (m_bIsMovingOnStairs)
            {
                 m_StairPointClimbStart = i_eStairPoint;
                 OnStairsExit();
                 return;
            }

            m_StairPointClimbStart = i_eStairPoint;

            m_bFloorCheckEnabled = false;

            m_bIsMovingOnStairs = true;
            m_Rigidbody.velocity = Vector2.zero;
        
            Vector3 LocalPosition = transform.localPosition;
            LocalPosition.x = i_StartPoint.x;
            transform.position = LocalPosition;

            m_TargetStairDirection = i_EndPoint.y > transform.position.y ? Vector3.up : Vector3.down;
        }
    }

    public void OnStairsExit()
    {
        m_bIsMovingOnStairs = false;
        m_bFloorCheckEnabled = true;
        Flip();
    }

    public void OnHit()
    {
        if (m_bIsDead)
        {
            return;
        }
        m_PhotonView.RPC("ChangeTeam", RpcTarget.AllBuffered, m_Rigidbody.position);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    
    [PunRPC]
    public void ChangeTeam(Vector2 diePosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            m_bIsDead = true;

            StartCoroutine(AfterDeath(diePosition));
        }
        
    }

    private IEnumerator AfterDeath(Vector3 diePosition)
    {
        yield return new WaitForSeconds(3f);
        
        Vector2 LocalPosition = diePosition;
        LocalPosition.x *= -1f;
        transform.localPosition = LocalPosition;
            
        m_TargetStairDirection = Vector3.zero;
        m_bIsMovingOnStairs = false;
        m_StairPointClimbStart = EStairPoint.None;
        m_TargetTeam = m_TargetTeam == ETeam.Team1 ? ETeam.Team2 : ETeam.Team1;
        
        m_bIsDead = false;
        
        SearchObjective();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_Rigidbody.velocity);
            stream.SendNext(m_bLookingRight);
        }
        else
        {
            //Network player, receive data
            m_Rigidbody.velocity = (Vector2)stream.ReceiveNext();
            bool direction = (bool) stream.ReceiveNext();
            if (direction != m_bLookingRight)
            {
                Flip();
            }
            m_bvaluesReceived = true;
            
        }
    }

    public void SetLookingRight(bool i_right)
    {
        if (!i_right)
        {
            Flip();
        }
        //m_bLookingRight = i_right;
    }
}
