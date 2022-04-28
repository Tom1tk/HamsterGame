using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    public bool gamePlaying;
    public int enemiesAlive, strawbCollected, strawbMax, enemiesMax;
    public TMP_Text enemyCounter, strawbCounter;
    public BallMovement PlayerRef;
    public Slider boostUI;
    public GameObject boostSlider;
    public Slider dodgeUI;
    public GameObject dodgeSlider;
    public doorOpen doorRef;
    public GameObject pauseMenu, winScreen, loseScreen;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gamePlaying = false;
        enemiesAlive = enemiesMax;
        strawbCollected = 0;
        PlayerRef = GameObject.Find("PlayerBall").GetComponent<BallMovement>();
        hideBoostUI();
        hideDodgeUI();
        hidePauseUI();
    }

    void FixedUpdate()
    {
        boostUI.value = PlayerRef.boostLv;
        dodgeUI.value = PlayerRef.dodgeTimer;
        enemyCounter.text = enemiesAlive.ToString() + " / " + enemiesMax.ToString();
        strawbCounter.text = strawbCollected.ToString() + " / " + strawbMax.ToString();

        if(strawbCollected == strawbMax)
        {
            showWinUI();
        }
        if((enemiesAlive <= 3) && (doorRef.open == false))
        {
            doorRef.openDoor();
        }
    }

    public void showBoostUI()
    {
        boostSlider.SetActive(true);
    }
    public void hideBoostUI()
    {
        boostSlider.SetActive(false);
    }
    public void showDodgeUI()
    {
        dodgeSlider.SetActive(true);
    }
    public void hideDodgeUI()
    {
        dodgeSlider.SetActive(false);
    }

    public void showPauseUI()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void hidePauseUI()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void showWinUI()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void showLoseUI()
    {
        loseScreen.SetActive(true);
        //Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
