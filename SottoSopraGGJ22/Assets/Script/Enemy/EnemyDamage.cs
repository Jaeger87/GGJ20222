using System;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyController m_Controller;

    private void Awake()
    {
        m_Controller = GetComponent<EnemyController>();
    }

    public void Hit()
    {
        Vector3 LocalPosition = transform.position;
        LocalPosition.x *= -1f;
        transform.localPosition = LocalPosition;

        m_Controller.OnHit();
    }
}
