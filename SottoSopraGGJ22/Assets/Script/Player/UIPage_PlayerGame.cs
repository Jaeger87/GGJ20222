using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPage_PlayerGame : MonoBehaviour
{
    [SerializeField]
    private Text NameLabel;

    public void SetNameLabelByTeam(ETeam i_Team)
    {
        SetNameLabel(i_Team == ETeam.Team1 ? "PLAYER 1" : "PLAYER 2");
    }
    
    public void SetNameLabel(string i_Name)
    {
        if (NameLabel == null)
        {
            return;
        }
        
        NameLabel.text = i_Name;
    }
}
