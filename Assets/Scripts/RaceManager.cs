using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public CheckPoint[] allCheckpoints;

    public int totalLaps;

    public CarController playerCar;
    public List<CarController> allAICar = new List<CarController>();
    public int playerPosition = 0;
    private float timeBetweenPosCheck = 0.2f, posCheckCounter = 0f;

    public float aiDefaultSpeed = 30f, playerDefaultSpeed = 30f, rubberBandSpeedMod = 3.5f, rubberBandAcceleration = 0.5f;

    public bool isStarting;
    public int currentCountdown = 3;
    private float startCounter;
    public float timeBetweenStartCount = 1f;

    public int playerStartPosition, aiNumberToSpawn;
    public Transform[] startPoints;

    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceCompleted;

    public string changeScene;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        totalLaps = RaceInfoManager.instance.noOfLaps;
        aiNumberToSpawn = RaceInfoManager.instance.noOfAI;

        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;
        UIManager.instance.countdownText.text = currentCountdown + "!";

        playerStartPosition = Random.Range(0, aiNumberToSpawn +1 );

        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitcher.instance.SetTarget(playerCar);

        //playerCar.transform.position = startPoints[playerStartPosition].position;
        //playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        for (int i = 0; i < aiNumberToSpawn +1 ; i++)
        {
            if(i!= playerStartPosition)
            {
                int selectedCars = Random.Range(0, carsToSpawn.Count);

                allAICar.Add(Instantiate(carsToSpawn[selectedCars], startPoints[i].position, startPoints[i].rotation));

                if(carsToSpawn.Count > aiNumberToSpawn - 1)
                {
                    carsToSpawn.RemoveAt(selectedCars);

                }
            }
        }

        UIManager.instance.playerPosText.text = (playerStartPosition + 1)+ "/" + (allAICar.Count + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting)
        {
            StartCountDown();
        }
        else
        {
            TrackPlayerPosition();
            RubberBanding();
        }

    }

    void TrackPlayerPosition()
    {
      posCheckCounter -= Time.deltaTime;
      if (posCheckCounter <= 0)
      {
          playerPosition = 1;

        foreach (CarController aiCar in allAICar)
        {
           if(aiCar.currentLap > playerCar.currentLap)
           {
              playerPosition++;
           }
           else if(aiCar.currentLap == playerCar.currentLap)
           {
            if(aiCar.nextCheckpoint > playerCar.nextCheckpoint)
            {
                        playerPosition++;
            }
            else if(aiCar.nextCheckpoint == playerCar.nextCheckpoint)
            {
              if(Vector3.Distance(aiCar.transform.position,allCheckpoints[aiCar.nextCheckpoint].transform.position)< Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position))
              {
                            playerPosition++;
              }
            }
           }
        }
            posCheckCounter = timeBetweenPosCheck;
            UIManager.instance.playerPosText.text = playerPosition + "/" + (allAICar.Count + 1);
      }
    }

    void RubberBanding()
    {
        if(playerPosition == 1)
        {
            foreach (CarController aiCar in allAICar)
            {
                aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeedMod, rubberBandAcceleration * Time.deltaTime);
            }

            playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod, rubberBandAcceleration * Time.deltaTime);
        }
        else
        {
            foreach (CarController aiCar in allAICar)
            {
                aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition/((float)allAICar.Count + 1))), rubberBandAcceleration * Time.deltaTime);
            }

            playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICar.Count + 1))), rubberBandAcceleration * Time.deltaTime);
        }
    }

    void StartCountDown()
    {

        startCounter -= Time.deltaTime;
        if (startCounter <= 0)
        {
            currentCountdown--;
            startCounter = timeBetweenStartCount;

            UIManager.instance.countdownText.text = currentCountdown + "!";

            if (currentCountdown == 0)
            {
                isStarting = false;

                UIManager.instance.countdownText.gameObject.SetActive(false);
                UIManager.instance.goText.gameObject.SetActive(true);
            }
        }

    }

    public void FinishRace()
    {
        raceCompleted = true;

        switch (playerPosition)
        {
            case 1:
                UIManager.instance.raceResultText.text = "You Finished 1st";
                if(RaceInfoManager.instance.trackToUnlock != "")
                {
                    if (!PlayerPrefs.HasKey(RaceInfoManager.instance.trackToUnlock + "_unlocked"))
                    {
                        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToUnlock + "_unlocked", 1);
                        UIManager.instance.trackUnlockedMessage.SetActive(true);
                    }
                }
                break;
            case 2:
                UIManager.instance.raceResultText.text = "You Finished 2nd";
                break;
            case 3:
                UIManager.instance.raceResultText.text = "You Finished 3rd";
                break;
            default:
                UIManager.instance.raceResultText.text = "You Finished " + playerPosition + "th";
                break;
        }

        UIManager.instance.raceResultScreen.SetActive(true);

        UIManager.instance.touchInput.SetActive(false);
        UIManager.instance.pauseMenuScreen.SetActive(false);

    }

    
   
}
