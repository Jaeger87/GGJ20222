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

    private ETeam m_TargetTeam = ETeam.Team1;

    private Rigidbody2D m_Rigidbody;
    
    private Vector2 m_FallCheckSize = new Vector2(0.1f, 0.1f);
    private Vector2 m_FrontWallCheckSize = new Vector2(0.1f, 0.3f);

    private bool m_bIsCollidingForward = false;
    private bool m_bGonnaFall = true;

    private bool m_bLookingRight = true;
    private bool m_bIsMovingOnStairs = false;

    private Vector3 m_ObjectivePosition = Vector3.zero;

    private Vector3 m_TargetStairDirection;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        SearchObjective();
    }

    private void SearchObjective()
    {
        GameObject Objective = GameObject.FindWithTag($"ControlPoint_{m_TargetTeam}");

        if (Objective != null)
        {
            m_ObjectivePosition = Objective.transform.position;
        }
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

        m_Rigidbody.isKinematic = m_bIsMovingOnStairs;
        
        if (m_bIsMovingOnStairs)
        {
            m_Rigidbody.MovePosition(transform.position + m_TargetStairDirection * Time.fixedDeltaTime * 2f);
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

    public void OnStairsEnter(Vector3 i_StartPoint, Vector3 i_EndPoint)
    {
        if (m_bIsMovingOnStairs)
        {
            m_bIsMovingOnStairs = false;
            return;
        }
        
        float DistanceFromStartToObjective = Vector3.Distance(i_StartPoint, m_ObjectivePosition);
        float DistanceFromEndToObjective = Vector3.Distance(i_EndPoint, m_ObjectivePosition);

        if (DistanceFromStartToObjective < DistanceFromEndToObjective)
        {
            return;
        }

        m_bIsMovingOnStairs = true;
        m_Rigidbody.velocity = Vector2.zero;
        
        Vector3 LocalPosition = transform.localPosition;
        LocalPosition.x = i_StartPoint.x;
        transform.position = LocalPosition;

        m_TargetStairDirection = i_EndPoint.y > transform.position.y ? Vector3.up : Vector3.down;
    }

    public void OnHit()
    {
        m_TargetStairDirection = Vector3.zero;
        m_bIsMovingOnStairs = false;
        m_TargetTeam = m_TargetTeam == ETeam.Team1 ? ETeam.Team2 : ETeam.Team1;
        SearchObjective();
    }
}
