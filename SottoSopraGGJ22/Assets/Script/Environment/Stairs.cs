using UnityEngine;

public class Stairs : MonoBehaviour
{
        
    [SerializeField]
    private Transform UpStartPoint = null;
    
    [SerializeField]
    private Transform DownStartPoint = null;
    
    [SerializeField]
    private LayerMask TargetMask;

    private Vector2 m_CheckSize = new Vector2(0.1f, 0.1f);

    private void Update()
    {
        Collider2D Hit = Physics2D.OverlapBox(UpStartPoint.position, m_CheckSize, 0, TargetMask);
        if (Hit != null)
        {
            EnemyController Enemy = Hit.gameObject.GetComponent<EnemyController>();

            if (Enemy != null)
            {
                Enemy.OnStairsEnter(UpStartPoint.position, DownStartPoint.position);
            }
        }
        
        Hit = Physics2D.OverlapBox(DownStartPoint.position, m_CheckSize, 0, TargetMask);
        if (Hit != null)
        {
            EnemyController Enemy = Hit.gameObject.GetComponent<EnemyController>();

            if (Enemy != null)
            {
                Enemy.OnStairsEnter(DownStartPoint.position, UpStartPoint.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(UpStartPoint != null)
            Gizmos.DrawWireCube(UpStartPoint.position, m_CheckSize);
        if(DownStartPoint != null)
            Gizmos.DrawWireCube(DownStartPoint.position, m_CheckSize);
    }
}
