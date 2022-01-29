using System;
using System.Collections;
using Photon.Pun;
using Script;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private PlayerDamage PlayerDamage;
    
    [SerializeField]
    private float MovementSpeed = 10f;
    
    [SerializeField]
    private float AirMovementSpeed = 8f;
    
    [SerializeField]
    private float JumpForce = 10f;

    [SerializeField]
    private float DashForce = 9.8f;

    [SerializeField]
    private float DashDeltaTime = 0.25f;

    [SerializeField] 
    private float JumpDownCollisionDisableTime = 0.25f;
    
    [SerializeField]
    private Transform HeadCheck = null;

    [SerializeField]
    private Transform FloorCheck = null;
    
    [SerializeField]
    private Transform FrontWallCheck = null;
    
    [SerializeField]
    private Transform BackWallCheck = null;

    [SerializeField]
    private LayerMask FloorMask;

    [SerializeField]
    private LayerMask WallMask;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip JumpSound;
    
    [SerializeField]
    private AudioClip DashSound;

    [SerializeField] 
    private float m_MapLimitLeft = 0;
    
    [SerializeField] 
    private float m_MapLimitRight = 0;
    
    [SerializeField] 
    private float m_MapLimitTop = 0;
    
    [SerializeField] 
    private float m_MapLimitBottom = 0;

    [Header("Effect")]
    [SerializeField]
    private GameObject DashParticles = null;
    
    private Rigidbody2D m_Rigidbody = null;

    private InputSystem.EMoveDirection m_Direction = InputSystem.EMoveDirection.Right;
    
    private bool m_bIsGrounded = false;
    private bool m_bHasFrontWall = false;
    private bool m_bHasBackWall = false;
    private bool m_bHasDash = true;
    
    private bool m_bIsJumping = false;
    private bool m_bIsDashing = false;
    private bool m_bCanJumpDownPlatform = false;
    private bool m_bIsGoingDown = false;
    private bool m_bCanJumpOverPlatform = false;

    private float m_TargetMovementSpeed = 0f;
    private float m_JumpDeltaTime = 0f;
    
    private Vector2 m_GroundCheckSize = new Vector2(0.1f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.5f);
    private Vector2 m_BackWallCheckSize = new Vector2(0.1f, 0.5f);

    private PlayerController m_Controller = null;
    private BoxCollider2D m_Collider;

    // Booleani networks
    private bool m_bNetworkPlayer = false;
    private bool m_bvaluesReceived = false;

    private bool DashAvailable => !m_bIsGrounded && m_JumpDeltaTime > DashDeltaTime;

    private PhotonView m_PhotonView = null;
    
    private bool m_bOffline => !PhotonNetwork.IsConnected;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Controller = GetComponent<PlayerController>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_PhotonView = GetComponent<PhotonView>();

        if (m_PhotonView.IsMine)
        {
            InputSystem.OnJumpEnter += OnJumpEnter;
            InputSystem.OnDashEnter += OnDashEnter;
        
            InputSystem.OnMoveHorizontalEnter += OnMoveHorizontal;
            InputSystem.OnMoveHorizontalUpdate += OnMoveHorizontal;
            InputSystem.OnMoveHorizontalExit += StopMovingHorizontal;
        
            InputSystem.OnMoveVerticalEnter += OnMoveVertical;
            InputSystem.OnMoveVerticalUpdate += OnMoveVertical;   
        }
    }

    private void OnDestroy()
    {
        if (m_PhotonView.IsMine)
        {
            InputSystem.OnJumpEnter -= OnJumpEnter;
            InputSystem.OnDashEnter -= OnDashEnter;

            InputSystem.OnMoveHorizontalEnter -= OnMoveHorizontal;
            InputSystem.OnMoveHorizontalUpdate -= OnMoveHorizontal;
            InputSystem.OnMoveHorizontalExit -= StopMovingHorizontal;

            InputSystem.OnMoveVerticalEnter -= OnMoveVertical;
            InputSystem.OnMoveVerticalUpdate -= OnMoveVertical;
        }
    }
    
    private void Update()
    {
        if (m_bNetworkPlayer)
        {
            return;
        }
        
        CheckGround();
            
        m_Controller.SetDashHintActive(DashAvailable);

        if (m_bIsDashing)
        {
            if (PlayerDamage.CheckHit())
            {
                OnHit();
                // m_bIsDashing = false;
            }
        }

        ClampPosition();
    }

    private void LateUpdate()
    {
        m_Collider.enabled = !m_bIsGoingDown;
        bool bMoving = m_Rigidbody.velocity.x != 0f;
        m_Animator.SetBool("Move", bMoving && !m_bIsJumping);
        m_Animator.SetBool("Jump", m_Rigidbody.velocity.y > 0 || m_bIsGoingDown);
        m_Animator.SetBool("Dash", m_bIsDashing);
    }
    
    private void ClampPosition()
    {
        Vector3 myPosition = transform.position;

        myPosition.x = Mathf.Max(m_MapLimitLeft, myPosition.x);
        myPosition.x = Mathf.Min(m_MapLimitRight, myPosition.x);
        myPosition.y = Mathf.Min(m_MapLimitTop, myPosition.y);
        myPosition.y = Mathf.Max(m_MapLimitBottom, myPosition.y);

        transform.position = myPosition;
    }

    private void OnMoveVertical(InputSystem.EMoveDirection i_Direction, float i_Axis)
    {
        if (i_Direction != InputSystem.EMoveDirection.Down)
        {
            return;
        }

        if (!m_bIsGrounded)
        {
            return;
        }

        if (m_bIsGoingDown || !m_bCanJumpDownPlatform)
        {
            return;
        }

        if (Mathf.Abs(i_Axis) < 0.5f)
        {
            return;
        } 
        
        m_bIsGoingDown = true;
        Invoke(nameof(ResetIsGoingDown), JumpDownCollisionDisableTime);
    }
    
    private void OnMoveHorizontal(InputSystem.EMoveDirection i_Direction, float i_Axis)
    {
        m_TargetMovementSpeed = (i_Direction == InputSystem.EMoveDirection.Left ? -1 : 1) * (m_bIsGrounded ? MovementSpeed : AirMovementSpeed);

        if (i_Direction != m_Direction)
        {
            if (!m_bOffline)
            {
                m_PhotonView.RPC("Flip", RpcTarget.AllBuffered);
            }
            else
            {
                m_Controller.Flip();
            }
        }

        m_Direction = i_Direction;
    }

    private void ResetIsGoingDown()
    {
        m_bIsGoingDown = false;
    }

    private void StopMovingHorizontal(InputSystem.EMoveDirection i_Direction, float i_Axis)
    {
        m_TargetMovementSpeed = 0f;
    }
    
    private void OnJumpEnter()
    {
        if (m_bCanJumpOverPlatform || m_bIsGrounded) {
            AddJumpToRigidBody();
        }
    }
    
    private void OnDashEnter()
    {
        if (!DashAvailable || m_bIsGrounded)
        {
            return;
        }

        AddDashToRigidBody();
        m_Controller.SendDash();
    }

    public void SetAsNetworkPlayer()
    {
        m_bNetworkPlayer = true;
    }

    private void OnHit()
    {
        // m_Animator.SetBool("Dash", true);
    }

    public void AddJumpToRigidBody()
    {
        m_bIsGrounded = false;
        m_bIsJumping = true;
        m_bvaluesReceived = true;
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, JumpForce);
        
        m_AudioSource.PlayOneShot(JumpSound);
    }
    
    public void AddDashToRigidBody()
    {
        m_bHasDash = false;
        m_bIsDashing = true;
        m_bvaluesReceived = true;
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -DashForce);
        
        m_AudioSource.PlayOneShot(DashSound);
    }

    [PunRPC]
    private void Flip()
    {
        m_Controller.Flip();
    }

    private void CheckGround()
    {
        if (FloorCheck != null)
        {
            Collider2D Hit = Physics2D.OverlapBox(FloorCheck.position, m_GroundCheckSize, 0, FloorMask);
            if(Hit != null)
            {
                OnGroundHit(Hit);
            }
            else
            {
                m_bIsGrounded = false;
                m_JumpDeltaTime += Time.deltaTime;
                m_Controller.OnCanJumpDownPlatform(false);
            }
        }
        
        if (HeadCheck != null)
        {
            Collider2D Hit = Physics2D.OverlapBox(HeadCheck.position, m_GroundCheckSize, 0, FloorMask);
            if(Hit != null)
            {
                m_Controller.OnCanJumpOverPlatform(true);
                m_bCanJumpOverPlatform = true;
            }
            else
            {
                m_bCanJumpOverPlatform = false;
            }
        }

        if (FrontWallCheck != null)
        {
            if(Physics2D.OverlapBox(FrontWallCheck.position, m_FrontWallCheckSize, 0, WallMask))
            {
                m_bHasFrontWall = true;
            }
            else
            {
                m_bHasFrontWall = false;
            }   
        }

        if (BackWallCheck != null)
        {
            if(Physics2D.OverlapBox(BackWallCheck.position, m_BackWallCheckSize, 0, WallMask))
            {
                m_bHasBackWall = true;
            }
            else
            {
                m_bHasBackWall = false;
            }   
        }
    }

    private void OnGroundHit(Collider2D i_Hit)
    {
        if (!m_bIsGrounded)
        {
            m_Animator.Play("Ground");
        }

        if (m_bIsDashing)
        {
            CreateDashEffect();
        }
        
        m_bIsGrounded = true;
        m_bIsDashing = false;
        m_bHasDash = true;
        m_bIsJumping = false;
        m_JumpDeltaTime = 0f;

        m_bCanJumpDownPlatform = i_Hit.CompareTag("Platform") && m_Rigidbody.velocity.y <= 0f;
        m_Controller.OnCanJumpOverPlatform(false);
        m_Controller.OnCanJumpDownPlatform(m_bCanJumpDownPlatform);
    }

    private void CreateDashEffect()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, FloorMask);

        if (hit != null)
        {
            var go = PhotonNetwork.Instantiate(DashParticles.gameObject.name, hit.point, DashParticles.transform.rotation);
            StartCoroutine(DestroyGO(go, 1f));
        }
    }

    private IEnumerator DestroyGO(GameObject go, float after)
    {
        yield return new WaitForSeconds(after);
        PhotonNetwork.Destroy(go);
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody == null)
        {
            return;
        }

        if(!m_bvaluesReceived)
        {
            m_Rigidbody.velocity = new Vector2(GetMovementForce(), m_Rigidbody.velocity.y);
        }

        m_bvaluesReceived = false;
    }

    private float GetMovementForce()
    {
        if (m_bHasFrontWall && m_Direction == InputSystem.EMoveDirection.Right)
        {
            return 0;
        }
        
        if (m_bHasBackWall && m_Direction == InputSystem.EMoveDirection.Left)
        {
            return 0;
        }

        float currentSpeed = m_Rigidbody.velocity.x;
        
        float targetSpeed = Mathf.Lerp(currentSpeed, m_TargetMovementSpeed, 0.2f);

        return targetSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(FloorCheck.position, m_GroundCheckSize);
        Gizmos.DrawWireCube(BackWallCheck.position, m_BackWallCheckSize);
        Gizmos.DrawWireCube(FrontWallCheck.position, m_FrontWallCheckSize);
        Gizmos.DrawWireCube(HeadCheck.position, m_FrontWallCheckSize);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_Rigidbody.velocity);
        }
        else
        {
            //Network player, receive data
            m_Rigidbody.velocity = (Vector2)stream.ReceiveNext();
            m_bvaluesReceived = true;
        }
    }
}
