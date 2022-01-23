using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField]
    private Transform DamageCheckPoint;

    [SerializeField]
    private Vector2 DamageCheckRange = new Vector2(0.1f, 0.1f);

    [SerializeField]
    private LayerMask HitLayer;

    public bool CheckHit()
    {
        if (DamageCheckPoint == null)
        {
            return false;
        }
        
        Collider2D Hit = Physics2D.OverlapBox(DamageCheckPoint.position, DamageCheckRange, 0, HitLayer);
        if (Hit != null)
        {
            EnemyDamage Enemy = Hit.GetComponent<EnemyDamage>();

            if (Enemy == null)
            {
                return false;
            }
            
            // hitted an enemy
            Enemy.Hit();
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (DamageCheckPoint == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireCube(DamageCheckPoint.position, DamageCheckRange);
    }
}
