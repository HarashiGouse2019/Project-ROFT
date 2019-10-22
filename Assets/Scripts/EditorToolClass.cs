using System;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class EditorToolClass : MonoBehaviour
{
    public static EditorToolClass Instance;

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
    private float timeInSamples;
    private float musicLengthInSamples;
    private float sampleDivisor = 1;
    private float seconds, minutes;

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

    private static DateTime lastModified;

    readonly private KeyCode tapKey = KeyCode.T;
    readonly private int tapLimit = 16;
    readonly private int doneTapLimit = 8;
    #endregion

    private void Awake()
    {
        Instance = this;
        LoadMusic();
    }

    private void Start()
    {
        if (record == true) InitialRecord();
    }

    // Update is called once per frame
    void Update()
    {

        if (musicSource != null)
            musicIsPlaying = musicSource.isPlaying;

        if (!tapCalulationDone) TapForBPM();
    }

    void FixedUpdate()
    {
        if (!caretIsBeingDragged) UpdateCaret();

        if (songTrackPosition.value > 1)
        {
            PauseMusic();
            musicSource.timeSamples = (int)musicLengthInSamples;
        }

        Tick();
        UpdatePulseOnBPMChange();
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

        songPercentage.text = (songTrackPosition.value * 100).ToString("F1", CultureInfo.InvariantCulture) + "%";

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

    public void TapForBPM()
    {
        if (Input.GetKeyDown(tapKey))
        {
            if (!musicIsPlaying)
                PlayMusic();
            else
            {
                //Add taps and check if max amount
                taps++;

                if (firstTickSample == 0) firstTickSample = musicSource.time;

                newPulse = (musicSource.time - firstTickSample) / taps;
                oldPulse = newPulse;

                if (taps > tapLimit - 1)
                {
                    UpdateBPM();
                    Debug.Log("BPM: " + bpm);
                }

                inactiveTaps = 0;
            }
        }

        if (inactiveTaps > doneTapLimit - 1) tapCalulationDone = true;
    }

    public void MarkTempo()
    {

    }

    public void MarkPosition()
    {

    }

    void Tick()
    {
        if ((musicSource.time - (determinedPulse * totalTicksSinceStart)) > determinedPulse)
        {

            if (taps > tapLimit - 1)
            {
                totalTicksSinceStart++;
                if (ticks == 1) AudioManager.audio.Play("FirstTick");
                else AudioManager.audio.Play("Tick");
                DetectInactivity();
            }
            else
            {
                totalTicksSinceStart = taps;
                if (ticks < (int)tickSignature)
                    ticks++;
                else
                    ticks = 1;
            }
        }
    }

    void LoadMusic()
    {
        musicSource = gameObject.AddComponent<AudioSource>();

        if (record)
            musicSource.clip = music;
        else
            musicSource.clip = MusicManager.manager.GetMusic(MapReader.Instance.m_name);

        musicLengthInSamples = musicSource.clip.length * musicSource.clip.frequency;
        musicSource.outputAudioMixerGroup = pitchMixer;

        //Check if we are recordimg

    }

    void TimeStamp()
    {
        if (seconds > 59.99f)
        {
            seconds = 0;
            minutes++;
        }
        else if (seconds < 0)
        {
            seconds = 59;
            minutes--;
        }

        seconds = musicSource.time - (60 * minutes);
    }


    void UpdatePulseOnBPMChange()
    {
        determinedPulse = 60 / bpm;
    }

    void UpdateBPM()
    {
        bpm = 60 / newPulse;
    }
    void DetectInactivity()
    {
        inactiveTaps++;
    }

    void InitialRecord()
    {
        CreateNewRFTM(music.name, Application.streamingAssetsPath + @"/");
    }

    public void CreateNewRFTM(string _name, string _path)
    {
        string rftmFileName = _name + ".rftm";
        string rftmFilePath = _path + rftmFileName;
        
        //Get our file format set
        //The Name
        //The Layout
        //The Author
        //When it was created
        //When it was modified
        //Sounds intense, but I'm just having fun

        if (!File.Exists(rftmFilePath))
        {
            Debug.Log("Creating new .rftm file...");
            using (StreamWriter rftmWriter = File.CreateText(rftmFilePath))
            {
                #region Author
                string user = System.Environment.UserName;
                #endregion

                #region Key Layout
                string layout = "";

                switch (Key_Layout.Instance.keyLayout)
                {
                    case Key_Layout.KeyLayoutType.Layout_1x4:
                        layout = "1 x 4";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_2x4:
                        layout = "2 x 4";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_3x4:
                        layout = "3 x 4";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_HomeRow:
                        layout = "Homerow";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_3Row:
                        layout = "Three Row";
                        break;
                    default:
                        break;
                }
                #endregion

                #region First Created
                DateTime firstCreated = File.GetCreationTime(rftmFilePath);
                #endregion

                #region Last Modified
                lastModified = firstCreated;
                #endregion //Although that you created it, the last modified value will be the same as creation time

                #region .rftm Information
                string[] rftmInformation = new string[]
                {
                   "[Author]\n" +  user,
                   "[Layout Type]\n" +  layout,
                   "[First Created On]\n" + firstCreated.ToLongDateString(),
                   "[Last Modified]\n" + lastModified.ToLongTimeString() + "\n",
                   "[Keys]\n"
                };
                #endregion

                for (int line = 0; line < rftmInformation.Length; line++)
                    rftmWriter.WriteLine(rftmInformation[line]);
            }
            Debug.Log(".rftm file created!");
        }
    }

    public void WriteToRFTM(string _name, string _path, string _data)
    {
        string rftmFileName = _name + ".rftm";
        string rftmFilePath = _path + rftmFileName;

        StreamWriter rftmWriter = File.AppendText(rftmFilePath);

        MapReader.Instance.m_name = _name;

        Debug.Log(rftmFilePath + " loaded");
        rftmWriter.WriteLine(_data);
        rftmWriter.Close();

        lastModified = File.GetLastWriteTime(rftmFilePath);
    }
}
