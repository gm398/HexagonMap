using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameTest");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialLevel");
    }
    public void LoadLevel1()
    {
        SceneManager.LoadScene("SimpleLevel");
    }
    public void LoadLevel2()
    {
        SceneManager.LoadScene("GameTest");
    }
    public void LoadSurvival()
    {
        SceneManager.LoadScene("SurvivalMode");
    }
    public void EndGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
