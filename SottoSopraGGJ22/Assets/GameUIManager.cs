using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        MatchManager.LeaveRoom();
    }
}