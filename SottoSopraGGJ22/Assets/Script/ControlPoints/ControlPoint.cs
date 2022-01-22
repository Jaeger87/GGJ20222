using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private ControlPointEventHandler ControlPointEH = null;

    private void Start()
    {
        GetEventHandler();
    }

    private void GetEventHandler()
    {
        ControlPointEH = FindObjectOfType<ControlPointEventHandler>();
        if (ControlPointEH != null)
        {
            ControlPointEH.ControlPointEvent.AddListener(ControlPointAttacked);
        }

    }

    private void ControlPointAttacked()
    {
        Debug.Log("Control Point Attacked");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ControlPointEH.ControlPointEvent.Invoke();
        }
    }
}

