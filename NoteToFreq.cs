using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Voice))]

public class NoteToFreq : MonoBehaviour
{
    public float internalFreqDebug = 0;

    public int baseNote                                 //input midi note public var
        {
        get { return _baseNote; }
        set { _baseNote = Mathf.Clamp(value, 0, 11); }
        }
        [SerializeField, Range(0, 11)]
        private int _baseNote;

    public int octave                                   //input octave public var
        {
        get { return _octave; }
        set { _octave = Mathf.Clamp(value, 0, 8); }
        }
        [SerializeField, Range(0, 8)]
        private int _octave;

    public static List<float> notes = new List<float>()                                                             //list of note base frequencies
        { 8.176f, 8.662f, 9.177f, 9.723f, 10.301f, 10.913f, 11.562f, 12.250f, 12.978f, 13.750f, 14.568f, 15.434f};
    public float noteMapped;                                                                                        //note after being mapped from midi note to frequency
    public static List<int> octaves = new List<int>()                                                               //octaves as base-frequency multipliers
        { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
    public int octaveMapped;                                                                                        //Octave after being mapped from midi octave to frequency
    public float noteFloatTofeedGenerator = 0;                                                                      //final frequency to send to synthvoice, after being mapped from note+octave.


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        noteMapped = notes[baseNote];       //map the input midi-basenote to a frequency

        octaveMapped = octaves[octave];     //map the input octave to a freq-multiplier int


        noteFloatTofeedGenerator = octaveMapped * noteMapped;                       //construct final frequency to send to voice, by multiplying the base note frequency with the octave(freq multiplier)

        gameObject.GetComponent<Voice>().newFrequency = noteFloatTofeedGenerator;   //apply the final frequency to the synth-voice

        internalFreqDebug = gameObject.GetComponent<Voice>().newFrequency;          //display debug info(final activated frequency)
    }
}
