using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private Text m_Text = null;

    public void SetLoadingText(string i_Text)
    {
        if (m_Text == null)
        {
            return;
        }

        m_Text.text = i_Text;
    }
}
