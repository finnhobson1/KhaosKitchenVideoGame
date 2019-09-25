using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicListener : MonoBehaviour
{

    public float MicLoudness;
    public bool isInitialized = false;

    private string _device;
    AudioClip _clipRecord;
    int _sampleWindow = 128;


    private void Start()
    {
        InitMic();
    }


    void InitMic()
    {
        if (_device == null) _device = Microphone.devices[0];
        _clipRecord = Microphone.Start(_device, true, 999, 44100);
    }


    void StopMicrophone()
    {
        Microphone.End(_device);
    }


    // Find max amplitude from audio clip
    float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); 
        _clipRecord.GetData(waveData, micPosition);
        // Find peak amplitude from the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        isInitialized = true;
        return levelMax;
    }


    void Update()
    {
        MicLoudness = LevelMax();
    }

    
    // Start mic when enabled by GameController
    void OnEnable()
    {
        InitMic();
    }

    // Stop mic when disabled by GameController
    void OnDisable()
    {
        MicLoudness = 0;
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }

}

