using Photon.Pun;
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
    private Transform FloorCheck = null;
    
    [SerializeField]
    private Transform FrontWallCheck = null;
    
    [SerializeField]
    private Transform BackWallCheck = null;

    [SerializeField]
    private LayerMask FloorMask;
    
    private Rigidbody2D m_Rigidbody = null;

    private bool m_bIsGrounded = false;
    private bool m_bHasFrontWall = false;
    private bool m_bHasBackWall = false;
    private bool m_bHasDash = true;
    private bool m_bLookingRight = true;
    private bool m_bIsJumping = false;
    private bool m_bIsDashing = false;

    private float m_TargetMovementSpeed = 0f;
    private float m_JumpDeltaTime = 0f;
    
    private Vector2 m_GroundCheckSize = new Vector2(0.4f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.5f);
    private Vector2 m_BackWallCheckSize = new Vector2(0.1f, 0.5f);

    private PlayerController m_Controller = null;

    // Booleani network
    private bool m_bNetworkPlayer = false;
    private bool m_bvaluesReceived = false;

    private bool DashAvailable => !m_bIsGrounded && m_JumpDeltaTime > DashDeltaTime;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Controller = GetComponent<PlayerController>();
    }


    public void SetAsNetworkPlayer()
    {
        m_bNetworkPlayer = true;
    }
    
    private void Update()
    {
        if (!m_bNetworkPlayer)
        {
            CheckMovement();
            CheckGround();
            CheckJump();
            CheckDash();
            
            m_Controller.SetDashHintActive(DashAvailable);

            if (m_bIsDashing)
            {
                if (PlayerDamage.CheckHit())
                {
                    OnHit();
                }
            }
        }
    }

    private void OnHit()
    {
        print("Player should hit");
    }
    
    private void CheckDash()
    {
        if (!m_bHasDash)
        {
            return;
        }
        
        bool dash = DashAvailable && Input.GetAxis("Vertical") < 0f;

        if (dash)
        {
            AddDashToRigidBody();
            m_Controller.SendDash();
        }
    }
    
    public void AddJumpToRigidBody()
    {
        m_bIsGrounded = false;
        m_bIsJumping = true;
        m_bvaluesReceived = true;
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, JumpForce);
    }
    
    public void AddDashToRigidBody()
    {
        m_bHasDash = false;
        m_bIsDashing = true;
        m_bvaluesReceived = true;
        // m_Rigidbody.AddForce(Vector2.down * DashForce, ForceMode2D.Impulse);
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -DashForce);
    }

    private void CheckMovement()
    {
        m_TargetMovementSpeed = Input.GetAxis("Horizontal") * (m_bIsGrounded ? MovementSpeed : AirMovementSpeed);
        float sign = Mathf.Sign(m_TargetMovementSpeed);

        if (m_TargetMovementSpeed != 0)
        {
            if (m_bLookingRight && sign < 0)
            {
                Flip();
            }

            if (!m_bLookingRight && sign > 0)
            {
                Flip();
            }
        }
        
        if (m_bHasFrontWall && m_TargetMovementSpeed > 0f)
        {
            m_TargetMovementSpeed = 0f;
        }

        if (m_bHasBackWall && m_TargetMovementSpeed < 0f)
        {
            m_TargetMovementSpeed = 0f;
        }
    }

    private void Flip()
    {
        m_Controller.Flip();
        m_bLookingRight = !m_bLookingRight;
    }

    private void CheckJump()
    {
        bool jump = !m_bIsJumping && Input.GetAxis("Jump") > 0.1f;
        
        if (m_bIsJumping && m_Rigidbody.velocity.y < 0)
        {
            m_bIsJumping = false;
        }
        

        if (jump && m_bIsGrounded)
        {
            AddJumpToRigidBody();
        }
    }

    private void CheckGround()
    {
        if (FloorCheck != null)
        {
            if(Physics2D.OverlapBox(FloorCheck.position, m_GroundCheckSize, 0, FloorMask))
            {
                m_bIsGrounded = true;
                m_bIsDashing = false;
                m_bHasDash = true;
                m_JumpDeltaTime = 0f;
            }
            else
            {
                m_bIsGrounded = false;
                m_JumpDeltaTime += Time.deltaTime;
            }
        }

        if (FrontWallCheck != null)
        {
            if(Physics2D.OverlapBox(FrontWallCheck.position, m_FrontWallCheckSize, 0, FloorMask))
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
            if(Physics2D.OverlapBox(BackWallCheck.position, m_BackWallCheckSize, 0, FloorMask))
            {
                m_bHasBackWall = true;
            }
            else
            {
                m_bHasBackWall = false;
            }   
        }
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody == null)
        {
            return;
        }
        if(!m_bvaluesReceived)
            m_Rigidbody.velocity = new Vector2(GetMovementForce(), m_Rigidbody.velocity.y);
        m_bvaluesReceived = false;
    }

    private float GetMovementForce()
    {
        float currentSpeed = m_Rigidbody.velocity.x;
        
        float targetSpeed = Mathf.Lerp(currentSpeed, m_TargetMovementSpeed, 0.2f);

        return targetSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(FloorCheck.position, m_GroundCheckSize);
        Gizmos.DrawWireCube(BackWallCheck.position, m_BackWallCheckSize);
        Gizmos.DrawWireCube(FrontWallCheck.position, m_FrontWallCheckSize);
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
