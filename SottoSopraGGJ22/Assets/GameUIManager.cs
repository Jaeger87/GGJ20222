using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Loading");
    }
}
