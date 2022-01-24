using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IPunObservable
{
    public enum EnemyMovement {
        Floor, Stairs
    };
    
    [SerializeField]
    private float MovementSpeed = 5f;

    [SerializeField]
    private Transform FrontWallCheck = null;
    
    [SerializeField]
    private LayerMask WallMask;

    [SerializeField]
    private Animator m_Animator = null;
    
    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip DieSound;
    
    [SerializeField]
    private AudioClip SpawnSound;

    private ETeam m_TargetTeam = ETeam.Team1;

    private EnemyMovement m_Movement = EnemyMovement.Floor;
    private int m_VerticalDirection = 1;

    private Rigidbody2D m_Rigidbody;
    
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.3f);

    private bool m_bIsCollidingForward = false;

    private bool m_bLookingRight = true;
    private bool m_bIsDead = false;
    
    private PhotonView m_PhotonView;

    private bool m_bOffline => !PhotonNetwork.IsConnected;

    private bool m_bValuesReceived = false;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        if (!m_bOffline && !PhotonNetwork.IsMasterClient)
        {
            m_Rigidbody.isKinematic = true;
        }
        m_PhotonView = GetComponent<PhotonView>();
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
        if (m_bOffline || PhotonNetwork.IsMasterClient)
        {
            m_Animator.SetBool("Climb", !m_bIsDead && m_Movement == EnemyMovement.Stairs);
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
            
            if (!m_bValuesReceived)
            {
                if (m_Movement == EnemyMovement.Floor)
                {
                    m_Rigidbody.velocity = new Vector2(
                        (m_bLookingRight ? transform.right : -transform.right).x * MovementSpeed,
                        m_Rigidbody.velocity.y);
                }
                else
                {
                    m_Rigidbody.velocity = new Vector2(
                        0,
                       MovementSpeed * m_VerticalDirection);
                }

            }
        }

        m_bValuesReceived = false;
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
        
        if (m_Movement == EnemyMovement.Stairs)
        {
            return;
        }
        
        if (FrontWallCheck != null)
        {
            if (Physics2D.OverlapBox(FrontWallCheck.position, m_FrontWallCheckSize, 0, WallMask))
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

    public void OnStairsEnter(bool i_bIsStartPoint, Stairs.EStairDirection i_Direction)
    {
        if(m_bOffline || PhotonNetwork.IsMasterClient)
        {
            if (m_Movement == EnemyMovement.Floor)
            {
                if (!i_bIsStartPoint)
                    return;
                m_Rigidbody.velocity = Vector2.zero;
                m_VerticalDirection = i_Direction == Stairs.EStairDirection.Down ? -1 : 1;
                m_Movement = EnemyMovement.Stairs;
                m_Rigidbody.isKinematic = true;

            }
            else if (m_Movement == EnemyMovement.Stairs)
            {
                if (i_bIsStartPoint)
                    return;
                m_Movement = EnemyMovement.Floor;
                m_Rigidbody.isKinematic = false;
                Flip();
            }
        }
    }

    public void OnHit()
    {
        if (m_bIsDead)
        {
            return;
        }

        if (m_bOffline)
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
        m_AudioSource.PlayOneShot(DieSound);
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
        
        m_TargetTeam = m_TargetTeam == ETeam.Team1 ? ETeam.Team2 : ETeam.Team1;
        
        m_bIsDead = false;
        
        m_AudioSource.PlayOneShot(SpawnSound);
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
            m_bValuesReceived = true;
            
        }
    }

    public void SetLookingRight(bool i_right)
    {
        if (!i_right)
        {
            Flip();
        }
    }
}
