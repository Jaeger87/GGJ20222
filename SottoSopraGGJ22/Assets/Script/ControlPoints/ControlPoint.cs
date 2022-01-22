using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private MatchManager MatchManager = null;
    [SerializeField] private ETeam i_Team;

    private void Start()
    {
        MatchManager = FindObjectOfType<MatchManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            MatchManager.GameEnded(i_Team);
        }
    }
}

