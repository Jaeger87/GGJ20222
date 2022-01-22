using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitHintController : MonoBehaviour
{
    [SerializeField]
    private LayerMask GroundLayer;

    private SpriteRenderer m_SpriteRender;
    
    private float m_YSize;
    
    private void Awake()
    {
        m_SpriteRender = GetComponent<SpriteRenderer>();
        if (m_SpriteRender != null)
        {
            m_YSize = GetComponent<SpriteRenderer>().bounds.size.y/2;
        }
    }

    private void Update()
    {
        Render();
    }

    private void Render()
    {
        RaycastHit2D Hit = Physics2D.Raycast(transform.parent.position, Vector2.down, 100f, GroundLayer);

        if (Hit != null)
        {
            Vector2 Position = Hit.point;
            Position.y += m_YSize/2f;
            
            transform.position = Position;
        }
    }
}
