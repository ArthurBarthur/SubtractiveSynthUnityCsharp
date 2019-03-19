using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NoteToFreq))]
public class Voice : MonoBehaviour
{
    //misc references
    public ChordSpawner parentChordSpawnerRef;

    //wave-gen stuff
    private double increment;
    private double phase;
    private readonly double sampling_frequency = 44100;
    public bool overrideWaveType = false;
    public ChordSpawner.WaveTypesSelector _wavetypeenmumfromchord;

    //volume stuff
    public float currentOscGain = 0f;
    public float targetOscGain = 0.1f;
    [Range(0, 1)]
    public float targetOscGainMP = 1f;
    public float oscGainLerpSpeed = 0.25f;

    //note stuff
    public float frequency = 220;
    private double finalOscGain = 0.1f;
    private float actualFrequency;
    public float newFrequency = 440;
    public float noteLerpSpeedInSec = 1f;

    //noise gen
    private System.Random randomNumber = new System.Random();
    private readonly float offset = 0.01f;

    //deltatime inside audiothread
    private double lastDSPtime;
    private double currentDSPtime;
    private double deltaDSPtime;

    //START / BEGIN PLAY
    private void Start()
    {
        gameObject.GetComponent<AudioSource>().volume   = targetOscGain;        //initialize volume on audiosource
        lastDSPtime                                     = AudioSettings.dspTime;//initialize audio tick delta calculation memory
    }

    //AUDIOTHREAD
    void OnAudioFilterRead(float[] data, int channels)
    {
       //find audio tick delta inside audio thread
        currentDSPtime = AudioSettings.dspTime;
        deltaDSPtime = currentDSPtime - lastDSPtime;
        lastDSPtime = currentDSPtime;

        // update increment in case frequency has changed
        increment = actualFrequency * 2.0 * System.Math.PI / sampling_frequency;
        //triangle helper
        double trianglehelp = actualFrequency * 2.0 / sampling_frequency;
            for (var i = 0; i < data.Length; i = i + channels)
            {
                phase = phase + increment;
            //select wave type
            switch (_wavetypeenmumfromchord)
                {
                    case ChordSpawner.WaveTypesSelector.Sine:
                    //generate sine wave
                    data[i] = (float)(System.Math.Sin(phase));
                    break;
                    case ChordSpawner.WaveTypesSelector.Triangle:
                    //generate triangle wave
                    double div = i * trianglehelp;
                        data[i] = (float)(((((int)div) % 2 == 0) ? -finalOscGain : finalOscGain) * (1.0 - 2.0 * (div - (int)div)));
                    break;
                    case ChordSpawner.WaveTypesSelector.Square:
                    //generate square wave
                    data[i] = Mathf.Sign(Mathf.Sin((float)phase * 2f * Mathf.PI));
                    break;
                    case ChordSpawner.WaveTypesSelector.Noise:
                    //generate noise wave
                    data[i] = offset - 1.0f + (float)randomNumber.NextDouble() * 2.0f;
                    break;
                }
                //make sure channel-summing doesn't blow stuff up
                if (channels == 2) data[i + 1] = data[i];
                //reset out-of-bounds wave-phase
                if (phase > 2 * System.Math.PI) phase = 0;
            }
        //send frequency changes async, from gamethread
        frequency = Mathf.MoveTowards(frequency, newFrequency, ((frequency + newFrequency) * (1 / noteLerpSpeedInSec)) * (float)deltaDSPtime);
        actualFrequency = frequency;
    }
    //GAMETHREAD
    private void Update()
    {
        if (overrideWaveType == false && _wavetypeenmumfromchord != parentChordSpawnerRef._waveTypesSelector) //locally override waveType of this voice?
        {
            _wavetypeenmumfromchord = parentChordSpawnerRef._waveTypesSelector;
        }
        //send volume changes
        currentOscGain                                  = Mathf.MoveTowards(currentOscGain, targetOscGain, oscGainLerpSpeed * Time.deltaTime);  //interp current gain towards target gain
        finalOscGain                                    = currentOscGain * targetOscGainMP;                                                     //multiply volume with
        gameObject.GetComponent<AudioSource>().volume   = (float)finalOscGain;                                                                  //set volume on audioSOURCE


    }
}