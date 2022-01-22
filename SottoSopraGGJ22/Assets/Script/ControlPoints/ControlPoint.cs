using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private MatchManager MatchManager = null;
    [SerializeField] private ETeam i_Team;
    [SerializeField] private int life = 0;

    private void Start()
    {
        MatchManager = FindObjectOfType<MatchManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //todo: Qui bisogna uccidere il nemico
            life--;
            if (life <= 0)
            {
                MatchManager.GameEnded(i_Team);
            }
            //todo: pensare anche a roba grafica
        }
    }
}

