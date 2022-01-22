using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private MatchManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static void GameEnded(ETeam i_Team)
    {
        Debug.Log($"{i_Team} lose");
        //@todo andrea devi capire come sincronizzare quest'evento
    }
}
