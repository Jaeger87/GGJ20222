using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField]
    private Transform DamageCheckPoint;

    [SerializeField]
    private Vector2 DamageCheckRange = new Vector2(0.5f, 0.1f);

    [SerializeField]
    private LayerMask HitLayer;

    public bool CheckHit()
    {
        if (DamageCheckPoint == null)
        {
            return false;
        }
        
        Collider2D[] Hits = Physics2D.OverlapBoxAll(DamageCheckPoint.position, DamageCheckRange, 0, HitLayer);
        if (Hits.Length > 0)
        {
            bool bHitted = false;
            foreach (var Hit in Hits)
            {
                EnemyDamage Enemy = Hit.GetComponent<EnemyDamage>();

                if (Enemy == null)
                {
                    continue;
                }

                bHitted = true;
                // hitted an enemy
                Enemy.Hit();
            }
            return bHitted;
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
