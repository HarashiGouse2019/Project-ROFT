using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

using static ROFTIOMANAGEMENT.RoftIO;

public class RoftPlayer : MonoBehaviour
{
    public static RoftPlayer Instance;

    #region Public Members
    //The EditorTool is going to be used to make creating music so much easier.
    //First, we'll get a reference of a song
    public AudioClip music;

    public bool record;

    public enum TickSignature
    {
        Normal = 4,
        Waltz = 3
    }

    public TickSignature tickSignature;

    //Then, we'll reference our UI
    public Button playButton;
    public Button pauseButton;
    public Button stopButton;

    //We'll then want to reference our time in samples text
    public TextMeshProUGUI samplesText;
    public TextMeshProUGUI songPercentage;


    //For the rest of the stuff like the Ui Slider and such
    public Slider songTrackPosition;

    //And the Dropdown for playback rate
    public TMP_Dropdown playbackRate;

    //Check if we're changing positions
    public bool caretIsBeingDragged;

    //And a pitch for play back speed;
    public AudioMixerGroup pitchMixer;

    public static AudioSource musicSource;
    #endregion

    #region Private Members
    private bool musicIsPlaying = false;
    private float musicLengthInSamples;


    //Recording and getting the bpm and then to get the pulse
    public int taps;
    public int inactiveTaps;

    public int ticks; //This will carry on the value of taps
    public int totalTicksSinceStart;
    public float firstTickSample; //The sample in which the designer started tapping

    public float oldPulse;
    public float newPulse;
    public float determinedPulse;
    public float bpm;
    public bool tapCalulationDone = false;

    public NoteEffector noteEffector;

    #endregion

    private void Awake()
    {
        
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
        LoadMusic();
    }

    private void Start()
    {
        if (record == false) StartCoroutine(Wait(3, () => PlayMusic()));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && record)
            PlayMusic();

        if (musicSource != null)
            musicIsPlaying = musicSource.isPlaying;
    }

    void FixedUpdate()
    {
        if (songTrackPosition != null && songTrackPosition.value > 1)
        {
            PauseMusic();
            musicSource.timeSamples = (int)musicLengthInSamples;
        }
    }

    void LoadMusic()
    {
        musicSource = gameObject.AddComponent<AudioSource>();

        if (record)
            musicSource.clip = RoftCreator.Instance.GetAudioFile();
        else
            musicSource.clip = MusicManager.Instance.GetMusic(MapReader.Instance.GetName());

        musicLengthInSamples = musicSource.clip.length * musicSource.clip.frequency;
        musicSource.outputAudioMixerGroup = pitchMixer;
    }

    //Play the song
    public void PlayMusic()
    {
        musicSource.Play();
        MusicManager.Instance.nowPlaying = musicSource.clip.name;
    }

    //Pause the song
    public void PauseMusic()
    {
        musicSource.Pause();
        if (!musicIsPlaying) musicSource.Play();
    }

    //Stop the Music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public float GetMusicLengthInSamples()
    {
        return musicLengthInSamples;
    }

    IEnumerator Wait(int duration, Action function = null)
    {
        yield return new WaitForSeconds(duration);
        if (function != null) function.Invoke();
    }
}