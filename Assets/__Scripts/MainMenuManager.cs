using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("__Golf_Scene_0");
    }
    public void StartProspector()
    {
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

}