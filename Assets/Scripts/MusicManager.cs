using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicPlayer;
    public AudioClip[] potentialClip;
    void Start()
    {
        musicPlayer.clip = potentialClip[Random.Range(0, potentialClip.Length)];
        musicPlayer.Play();
    }

    
}
