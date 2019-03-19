using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChordSpawner : MonoBehaviour
{
    public GameObject voicePrefab;
    private List<GameObject> createdVoices = new List<GameObject>();
    [Range(1, 3)]
    public int numberOfVoices;

    private readonly List<int> chordAmin = new List<int>() { 9, 0, 4 };
    private readonly List<int> chordAmaj = new List<int>() { 9, 1, 4 };
    private readonly List<int> chordBmin = new List<int>() { 11, 2, 6 };
    private readonly List<int> chordCmaj = new List<int>() { 0, 4, 7 };
    private readonly List<int> chordDmin = new List<int>() { 2, 5, 9 };
    private readonly List<int> chordDmaj = new List<int>() { 2, 6, 9 };
    private readonly List<int> chordEmin = new List<int>() { 4, 7, 11 };
    private readonly List<int> chordFmaj = new List<int>() { 5, 9, 0 };
    private readonly List<int> chordGmaj = new List<int>() { 7, 11, 2 };

    public enum WaveTypesSelector { Sine, Triangle, Square, Noise};
    public WaveTypesSelector _waveTypesSelector;

    public enum ChordSelector { Amin, Amaj, Bmin, Cmaj, Dmin, Dmaj, Emin, Fmaj, Gmaj };
    public ChordSelector _chordSelector;
    private List<int> activeChord = new List<int>() { 0, 0, 0 };

    public int octave = 3;
    [Range(0, 1)]
    public float volume = 0.1f;
    private bool isBassNoteLowest;
    public float lowPassFilterFreq
    {
        get { return _lowPassFilterFreq; }
        set { _lowPassFilterFreq = Mathf.Clamp(value, 0, 22000); }
    }
    [SerializeField, Range(0, 22000)]
    private float _lowPassFilterFreq = 22000;

    // Start is called before the first frame update
    void Start()
    {
        SpawnVoices();                                                      //spawn in prefab gameobjects with sine-generator and note-switcher
        print(this.name + "'s voiceCount is: " + createdVoices.Count);      //print number of spawned voices to log
    }

    // Update is called once per frame
    void Update()
    {
        MakeChordSelection();       //select active chord
        CheckIfBassNoteLowest();    //check if the defining note(bass, indexed at 0) is the lowest note in chord
        UpdateVoices();             //update voices
    }

    //spawn(instantiate) the voice prefabs in the world
    public void SpawnVoices()
    {
        for (int i = 0; i < (numberOfVoices); i++)
        {
            GameObject _newVoice = (GameObject)Instantiate(voicePrefab);    //instantiate a voice
            createdVoices.Add(_newVoice);                                   //add the new voice to our createdVoices-list
            _newVoice.GetComponent<Voice>().parentChordSpawnerRef = this;   //add reference to this spawner-parent inside voice
        }
    }

    //select active chord(list of notes) according to the chordselector enum
    void MakeChordSelection()
    {
       switch (_chordSelector)
        {
            case ChordSelector.Amin:
                activeChord = chordAmin;
                break;
            case ChordSelector.Amaj:
                activeChord = chordAmaj;
                break;
            case ChordSelector.Bmin:
                activeChord = chordBmin;
                break;
            case ChordSelector.Cmaj:
                activeChord = chordCmaj;
                break;
            case ChordSelector.Dmin:
                activeChord = chordDmin;
                break;
            case ChordSelector.Dmaj:
                activeChord = chordDmaj;
                break;
            case ChordSelector.Emin:
                activeChord = chordEmin;
                break;
            case ChordSelector.Fmaj:
                activeChord = chordFmaj;
                break;
            case ChordSelector.Gmaj:
                activeChord = chordGmaj;
                break;
        }
    }

    //check if the bass-note (0 in chordlist) is lower than the other notes
    void CheckIfBassNoteLowest()
    {
        if ((activeChord[0] < activeChord[1]) || (activeChord[0] < activeChord[2]))
        {
            isBassNoteLowest = true;  //is lowest, set isBassNoteLowest-bool to true(this will tell the script to not do anything to the note later, as it is where it should be)
        }
        else
        {
            isBassNoteLowest = false;   //is not lowest - set isBassNoteLowest-bool to false(this will tell the script to subtract an octave from the note so it is lowest)
        }
    }

    //Update voice gameobjects
    void UpdateVoices()
    {
        for (int i = 0; i < 3; i++)                 
        {
            if ((createdVoices.Count - 1) >= i)                                                             //if (CreatedVoices[i] != null)
            {
                createdVoices[i].GetComponent<AudioLowPassFilter>().cutoffFrequency = _lowPassFilterFreq;   //send LowPassFilterValue to voices
                createdVoices[i].GetComponent<NoteToFreq>().baseNote = activeChord[i];                      //send notes to voices
                createdVoices[i].GetComponent<Voice>().targetOscGain = volume;                              //send volume to voices
                if (isBassNoteLowest == false && i == 0)                                                    //check if bassnote is lowest and send proper octave to voices
                {
                    createdVoices[i].GetComponent<NoteToFreq>().octave = octave - 1;                        //set defining note one octave lower if it isn't the lowest one in the chord
                }
                else
                {
                    createdVoices[i].GetComponent<NoteToFreq>().octave = octave;                            //Sets the target octave voices when no changes are needed
                }
            }
        }
    }
}