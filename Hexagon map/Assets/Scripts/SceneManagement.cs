using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
 

    public void StartGame()
    {
        SceneManager.LoadScene("GameTest");
    }
    public void EndGame()
    {
        SceneManager.LoadScene("MainManu");
    }
}
