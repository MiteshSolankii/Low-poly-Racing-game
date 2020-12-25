using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text lapCounterText, currentLapTimeText, bestLapTimeText, playerPosText, countdownText, goText, raceResultText;
    public GameObject raceResultScreen, pauseMenuScreen, trackUnlockedMessage,touchInput;
    public string changeScene;
    public bool isPaused;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        pauseMenuScreen.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            touchInput.SetActive(false);
         
        }
        else
        {
            Time.timeScale = 1f;
            touchInput.SetActive(true);
        }

    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(changeScene);
    }

    public void QuitGame()
    {
        Application.Quit();
        
    }
}
