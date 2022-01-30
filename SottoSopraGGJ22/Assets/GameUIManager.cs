using Photon.Pun;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_PauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void BackToMainMenu()
    {
        MatchManager.LeaveRoom();
    }

    public void TogglePauseMenu()
    {
        if (!MatchManager.IsGameStarted)
        {
            return;
        }
        
        if (m_PauseMenu != null)
        {
            m_PauseMenu.gameObject.SetActive(!m_PauseMenu.gameObject.activeSelf);
        }
    }
}