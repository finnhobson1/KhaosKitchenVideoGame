
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicPlayer : MonoBehaviour
{
    public GameController gameController;

        

    //GameObjects
    private AudioSource BackgroundSource;
    public AudioClip BackgroundMusic;
    public AudioClip GameOverClip;
    public AudioClip[] RoundBreaks;
    public AudioClip[] Countdown;
    public AudioClip TenSecondCountdown;

    private float VolumeOneShot = 1f;
    private float VolumeLoud = 5f;

    //Primitives
    private int NumberOfRoundBreaks;
    private int Switch = 0;
    
    //Booleans
    private bool isRoundPaused = false;


    public bool IsRoundPaused
    {
        get
        {
            return isRoundPaused;
        }
    }

    private bool isMusicPaused = false;

    public bool IsMusicPaused
    {
        get
        {
            return isMusicPaused;
        }
    }

    private void Awake()
    {
        BackgroundSource = GetComponent<AudioSource>();
        BackgroundSource.clip = BackgroundMusic;
    }

    private void Start()
    {
        NumberOfRoundBreaks = RoundBreaks.Length;
    }
    
    public void PauseMusic()
    {
        //isMusicPaused = true;
        BackgroundSource.Stop();
    }

    public void UnPauseMusic()
    {
        //isMusicPaused = false;
        BackgroundSource.UnPause();
    }

    public void PlayGameOver()
    {
        BackgroundSource.Stop();
        BackgroundSource.PlayOneShot(GameOverClip, VolumeOneShot);
    }

    void PlayBackgroundMusic()
    {
        BackgroundSource.Play();
    }

    public void PlayRoundBreak()
    {
//        if(!isMusicPaused) PauseMusic();
        BackgroundSource.PlayOneShot(RoundBreaks[Switch], VolumeOneShot);
        Switch = (Switch + 1) % NumberOfRoundBreaks;
    }

    public void StartRoundMusic()
    {
        //isRoundPaused = false;
        //isMusicPaused = false;
        PlayBackgroundMusic();
    }

    public void PlayCountDown(int count)
    {
        BackgroundSource.PlayOneShot(Countdown[count], VolumeOneShot);
    }

    public void PlayTenSecondCountdown()
    {
        BackgroundSource.PlayOneShot(TenSecondCountdown, VolumeLoud);
    }
}
