using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void startGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void quitGame()
    {
        Application.Quit();
    }
    
}
