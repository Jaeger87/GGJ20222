using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private float MovementSpeed = 10f;
    
    [SerializeField]
    private float AirMovementSpeed = 8f;
    
    [SerializeField]
    private float JumpForce = 10f;

    [SerializeField]
    private float DashForce = 9.8f;
    
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
    private float m_TargetMovementSpeed = 0f;

    private Vector2 m_GroundCheckSize = new Vector2(0.4f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.5f);
    private Vector2 m_BackWallCheckSize = new Vector2(0.1f, 0.5f);

    private PlayerController m_Controller = null;

    // Booleani network
    private bool m_bNetworkPlayer = false;
    private bool m_bvaluesReceived = false;

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
        }
        
    }

    private void CheckDash()
    {
        if (!m_bHasDash)
        {
            return;
        }
        
        bool dash = !m_bIsGrounded && m_Rigidbody.velocity.y < 2f && Input.GetAxis("Vertical") < 0f;

        if (dash)
        {
            AddDashToRigidBody();
            
            m_Controller.SendDash();
        }
    }

    public void AddDashToRigidBody()
    {
        m_bHasDash = false;
        m_bvaluesReceived = true;
        m_Rigidbody.AddForce(Vector2.down * DashForce, ForceMode2D.Impulse);
    }
    
    public void AddJumpToRigidBody()
    {
        m_bIsGrounded = false;
        m_bIsJumping = true;
        m_bvaluesReceived = true;
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, JumpForce);
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
            }
            else
            {
                m_bIsGrounded = false;
                m_bHasDash = true;
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
