using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public enum DirectionToGO {LEFT, RIGHT, NOSTAIRS}

public enum Floor {ZERO = 0, ONE = 1, TWO = 2, LAST = 3}

public class EnvironmentContainer : MonoBehaviour
{
    [SerializeField] 
    private List<Stairs> Team1Stars;
    [SerializeField] 
    private List<Stairs> Team2Stars;

    public DirectionToGO MostNearStairs(Vector3 i_position, ETeam i_team, Floor i_floor)
    {
        List<Stairs> stairsToCheck = i_team == ETeam.Team1 ? Team1Stars : Team2Stars;

        Stairs mostNearStair = null;
        float mostXNear = float.MaxValue;
        for (int i = 1; i < stairsToCheck.Count; i++)
        {
            Stairs toCheck = stairsToCheck[i];
            if (i_floor != toCheck.Floor)
                continue;
            if (mostNearStair != null)
            {
                float xDifference = Math.Abs(i_position.x - toCheck.transform.position.x);
                if (xDifference < mostXNear)
                {
                    mostNearStair = stairsToCheck[i];
                    mostXNear = Math.Abs(i_position.x - toCheck.transform.position.x);
                }
            }
            else
            {
                mostNearStair = stairsToCheck[i];
                mostXNear = Math.Abs(i_position.x - toCheck.transform.position.x);
            }
            
        }

        if (mostNearStair == null)
            return DirectionToGO.NOSTAIRS;
        return mostNearStair.transform.position.x > i_position.x ? DirectionToGO.RIGHT : DirectionToGO.LEFT;
    }
}
