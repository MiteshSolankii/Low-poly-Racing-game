﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceInfoManager : MonoBehaviour
{
    public static RaceInfoManager instance;
    public CarController racerToUse;
    public string trackToLoad;
    public int noOfAI;
    public int noOfLaps;

    public bool enteredRace;

    public Sprite trackSprite, racerSprite;

    public string trackToUnlock;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("deleted");
        }
       */
    }
}
