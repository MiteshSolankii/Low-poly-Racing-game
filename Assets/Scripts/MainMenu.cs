using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel;
    public Image trackSelectImage, racerSelectImage;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (RaceInfoManager.instance.enteredRace)
        {
            trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
            racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetup();
        }

        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        RaceInfoManager.instance.enteredRace = true;
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);
    }
    public void QuitGame()
    {
        Application.Quit();
        
    }

    public void OpenRaceSetup()
    {
        raceSetupPanel.SetActive(true);
    }
    public void CloseRaceSetup()
    {
        raceSetupPanel.SetActive(false);
    }
    public void OpenTrackSelect()
    {
        
        CloseRaceSetup();
        AudioManager.instance.PlaySound("selectCourse");
        trackSelectPanel.SetActive(true);
    }
    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }
    public void OpenRacerSelect()
    {
        CloseRaceSetup();
        AudioManager.instance.PlaySound("selectVehicle");
        racerSelectPanel.SetActive(true);
    }
    public void CloseRacerSelect()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }
}
