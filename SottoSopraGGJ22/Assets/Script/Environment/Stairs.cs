using System;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public enum EStairDirection
    {
        Down, Up
    }
    
    [SerializeField]
    private Transform StartPoint = null;
    
    [SerializeField]
    private Transform EndPoint = null;
    
    [SerializeField]
    private LayerMask TargetMask;

    [SerializeField]
    private EStairDirection m_Direction = EStairDirection.Up;
    
    private Vector2 m_CheckSize = new Vector2(0.01f, 0.01f);
    
    const float distanceTollerance = 0.25f;
    private void Update()
    {
        Collider2D Hit = Physics2D.OverlapBox(StartPoint.position, m_CheckSize, 0, TargetMask);
        if (Hit != null) {
            EnemyController Enemy = Hit.gameObject.GetComponent<EnemyController>();
            float distance = Vector3.Distance(StartPoint.position, Hit.transform.position);
            if (Enemy != null && distance < distanceTollerance)
                Enemy.OnStairsEnter(true, m_Direction);
        }
        Hit = Physics2D.OverlapBox(EndPoint.position, m_CheckSize, 0, TargetMask);
        if (Hit != null) {
            EnemyController Enemy = Hit.gameObject.GetComponent<EnemyController>();
            float distance = Vector3.Distance(EndPoint.position, Hit.transform.position);
            if (Enemy != null && distance < distanceTollerance)
                Enemy.OnStairsEnter(false, m_Direction);
        }
    }

    private void OnDrawGizmos()
    {
        if(StartPoint != null)
            Gizmos.DrawWireCube(StartPoint.position, m_CheckSize);
        if(EndPoint != null)
            Gizmos.DrawWireCube(EndPoint.position, m_CheckSize);
    }
}
