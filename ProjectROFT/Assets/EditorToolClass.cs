using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class EditorToolClass : MonoBehaviour
{
    #region Public Members
    //The EditorTool is going to be used to make creating music so much easier.
    //First, we'll get a reference of a song
    public AudioClip music;

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
    #endregion

    private AudioSource musicSource;
    private bool musicIsPlaying = false;
    private float timeInSamples;
    private float musicLengthInSamples;
    private float sampleDivisor = 1;
    private float seconds, minutes;

    // Start is called before the first frame update
    void Start()
    {
        LoadMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (musicSource != null)
            musicIsPlaying = musicSource.isPlaying;
    }

    void FixedUpdate()
    {
        if (!caretIsBeingDragged) UpdateCaret();

        if (songTrackPosition.value > 1)
        {
            PauseMusic();
            musicSource.timeSamples = (int)musicLengthInSamples;
        }

        TimeStamp();
    }

    //Play the song
    public void PlayMusic()
    {
        Debug.Log("Play");
        musicSource.Play();
    }

    //Pause the song
    public void PauseMusic()
    {
        Debug.Log("Pause");
        musicSource.Pause();
        if (!musicIsPlaying) musicSource.Play();
    }

    //Stop the Music
    public void StopMusic()
    {
        Debug.Log("Stop");
        musicSource.Stop();
    }

    public void UpdateCaret()
    {

        //So, here's how this is going to work...
        //The Track Position is between values 0 and 1
        //We have to find a way to convers a percentage of something to samples...
        musicSource.pitch = sampleDivisor;

        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1 / musicSource.pitch);

        timeInSamples = musicSource.timeSamples;

        songTrackPosition.value = timeInSamples / musicLengthInSamples;

        songPercentage.text = (songTrackPosition.value * 100).ToString("F1", CultureInfo.InvariantCulture) + "%";

        samplesText.text = minutes.ToString() + ":" + Mathf.FloorToInt(seconds).ToString("D2");
    }

    public void UpdateSampleFromCaretPosition()
    {
        Debug.Log("HEWWO!!!!");
        ToggleCaretDrag(true);

        //We're doing the opposite of the UpdateCaret
        musicSource.timeSamples = (int)(songTrackPosition.value * musicLengthInSamples);

        timeInSamples = musicSource.timeSamples;

        songPercentage.text = (songTrackPosition.value * 100).ToString("F1",CultureInfo.InvariantCulture) + "%";

        samplesText.text = minutes.ToString() + ":" + Mathf.FloorToInt(seconds).ToString();
    }

    public void UpdatePlayBackSpeed()
    {
        for (int option = 0; option < playbackRate.options.Count; option++)
        {
            if (option == playbackRate.value)
            {
                switch (option)
                {
                    case 0:
                        sampleDivisor = 1;
                        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("fftSize", 927f);
                        break;
                    case 1:
                        sampleDivisor = 0.75f;
                        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("fftSize", 1060f);
                        break;
                    case 2:
                        sampleDivisor = 0.5f;
                        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("fftSize", 3608f);
                        break;
                    case 3:
                        sampleDivisor = 0.25f;
                        break;
                }
            }
        }
    }

    public void ToggleCaretDrag(bool _on)
    {
        caretIsBeingDragged = _on;
    }

    void LoadMusic()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = music;
        musicLengthInSamples = musicSource.clip.length * musicSource.clip.frequency;
        musicSource.outputAudioMixerGroup = pitchMixer;
    }

    void TimeStamp()
    {


        if (seconds > 59.99f)
        {
            seconds = 0;
            minutes++;
        } else if (seconds < 0)
        {
            seconds = 59;
            minutes--;
        }
        
        seconds = musicSource.time - (60 * minutes);
    }
}
