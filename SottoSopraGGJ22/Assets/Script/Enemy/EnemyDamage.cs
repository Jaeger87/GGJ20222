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
        m_Controller.OnHit();
    }
}
