using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float MovementSpeed = 5f;
    
    [SerializeField]
    private Transform FallCheck = null;
    
    [SerializeField]
    private Transform FrontWallCheck = null;
    
    [SerializeField]
    private LayerMask FloorMask;

    private Rigidbody2D m_Rigidbody;
    
    private Vector2 m_FallCheckSize = new Vector2(0.1f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.3f);

    private bool m_bIsCollidingForward = false;
    private bool m_bGonnaFall = true;

    private bool m_bLookingRight = true;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckCollisions();
        
        if (m_bIsCollidingForward || m_bGonnaFall)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody == null)
        {
            return;
        }

        m_Rigidbody.velocity = new Vector2((m_bLookingRight ? transform.right : -transform.right).x * MovementSpeed, m_Rigidbody.velocity.y);
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
        if (FallCheck != null)
        {
            if (Physics2D.OverlapBox(FallCheck.position, m_FallCheckSize, 0, FloorMask))
            {
                m_bGonnaFall = false;
            }
            else
            {
                m_bGonnaFall = true;
            }
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
        Gizmos.DrawWireCube(FallCheck.position, m_FallCheckSize);
        Gizmos.DrawWireCube(FrontWallCheck.position, m_FrontWallCheckSize);
    }
}
