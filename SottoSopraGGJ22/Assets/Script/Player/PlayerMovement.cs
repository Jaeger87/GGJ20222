using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    
    private bool m_bIsJumping = false;
    private float m_TargetMovementSpeed = 0f;

    private Vector2 m_GroundCheckSize = new Vector2(0.4f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.5f);
    private Vector2 m_BackWallCheckSize = new Vector2(0.1f, 0.5f);

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        CheckMovement();
        CheckGround();
        CheckJump();
        CheckDash();
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
            m_bHasDash = false;
            m_Rigidbody.AddForce(Vector2.down * DashForce, ForceMode2D.Impulse);
        }
    }

    private void CheckMovement()
    {
        m_TargetMovementSpeed = Input.GetAxis("Horizontal") * (m_bIsGrounded ? MovementSpeed : AirMovementSpeed);
        if (m_bHasFrontWall && m_TargetMovementSpeed > 0f)
        {
            m_TargetMovementSpeed = 0f;
        }

        if (m_bHasBackWall && m_TargetMovementSpeed < 0f)
        {
            m_TargetMovementSpeed = 0f;
        }
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
            m_bIsGrounded = false;
            m_bIsJumping = true;
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, JumpForce);
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
        
        m_Rigidbody.velocity = new Vector2(GetMovementForce(), m_Rigidbody.velocity.y);
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
}
