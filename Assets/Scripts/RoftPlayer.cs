using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

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
        if (record == true) InitialRecord();
        else StartCoroutine(Wait(3, () => PlayMusic()));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            PlayMusic();

        if (musicSource != null)
            musicIsPlaying = musicSource.isPlaying;
    }

    void FixedUpdate()
    {
        if (songTrackPosition.value > 1)
        {
            PauseMusic();
            musicSource.timeSamples = (int)musicLengthInSamples;
        }
    }

    void LoadMusic()
    {
        musicSource = gameObject.AddComponent<AudioSource>();

        if (record)
            musicSource.clip = music;
        else
            musicSource.clip = MusicManager.Instance.GetMusic(MapReader.Instance.m_name);

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

    void InitialRecord()
    {
        CreateNewRFTM(music.name, Application.streamingAssetsPath + @"/");
    }

    int GenerateROFTID()
    {
        /*ROFTID is as structured:
         * R-yy-dd-mm-(100 to 999)
         * Example: R-180512841
         * 
         * R - ROFTID
         * 18 - 2018
         * 05 - 5th
         * 12 - Dec
         * Number randomized to 841
         */

        int[] numRange =
        {
            100,
            999
        };

        string stringROFTID = "";

        DateTime date = DateTime.Now;

        string yearDigit = date.ToString("yy");
        string dayDigit = date.ToString("dd");
        string monthDigit = date.ToString("mm");
        int uniqueNum = Random.Range(numRange[0], numRange[1]);

        stringROFTID += (yearDigit + dayDigit + monthDigit + uniqueNum.ToString());

        return Convert.ToInt32(stringROFTID);
    }

    int GenerateGROUPID()
    {
        /*GROUPID is as structured:
         * G-yy-dd-mm-(1000 to 9990)
         * Example: G-1805121240
         * 
         * G - GROUPID
         * 18 - 2018
         * 05 - 5th
         * 12 - Dec
         * Number randomized to 1240
         */

        int[] numRange =
        {
            1000,
            9999
        };

        string stringROFTID = "";

        DateTime date = DateTime.Now;

        string yearDigit = date.ToString("yy");
        string dayDigit = date.ToString("dd");
        string monthDigit = date.ToString("mm");
        string uniqueNum = Random.Range(numRange[0], numRange[1]).ToString();

        stringROFTID += (yearDigit + dayDigit + monthDigit + uniqueNum);

        return Convert.ToInt32(stringROFTID);
    }

    public void CreateNewRFTM(string _name, string _path)
    {
        string rftmFileName = _name + ".rftm";
        string rftmFilePath = _path + rftmFileName;
        
        if (!File.Exists(rftmFilePath))
        {
            Debug.Log("Creating new .rftm file...");
            using (StreamWriter rftmWriter = File.CreateText(rftmFilePath))
            {
                const string newLine = "\n";

                #region General
                string t_general = "[General]\n";
                string p_Author = "Author: " + System.Environment.UserName + newLine;
                string p_AudioFileName = "AudioFilename: " + newLine;
                string p_Gamemode = "";
                switch (GameManager.Instance.gameMode)
                {
                    case GameManager.GameMode.STANDARD:
                        p_Gamemode = "GameMode: " + "Standard" + newLine;
                        break;

                    case GameManager.GameMode.TECHMEISTER:
                        p_Gamemode = "GameMode: " + "Techmeister" + newLine;
                        break;

                    case GameManager.GameMode.TBR_HOMEROW:
                        p_Gamemode = "GameMode: " + "Type-By-Region(Homerow)" + newLine;
                        break;

                    case GameManager.GameMode.TBR_ALL:
                        p_Gamemode = "GameMode: " + "Type-By-Region(Allrows)" + newLine;
                        break;

                    default:
                        break;
                }
                #endregion

                #region Metadata
                string t_metadata = "[Metadata]\n";
                string p_Title = "Title: " + newLine;
                string p_TitleUnicode = "TitleUnicode: " + newLine;
                string p_Artist = "Artist: " + newLine;
                string p_ArtistUnicode = "ArtistUnicode: " + newLine;
                string p_Creator = "Creator: " + newLine;
                string p_ROFTID = "ROFTID: R-" + GenerateROFTID() + newLine;
                string p_GROUPID = "GROUPID: G-" + GenerateGROUPID() + newLine;
                #endregion

                #region Difficulty
                string t_difficulty = "[Difficulty]\n";
                string p_StressBuild = "StressBuild: " + GameManager.Instance.stressBuild.ToString() + newLine;
                string p_ObjectCount = "ObjectCount: " + newLine;
                string keyInfo = "";
                #region Key Count

                switch (Key_Layout.Instance.keyLayout)
                {
                    case Key_Layout.KeyLayoutType.Layout_1x4:
                        keyInfo = "4";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_2x4:
                        keyInfo = "8";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_3x4:
                        keyInfo = "12";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_HomeRow:
                        keyInfo = "10";
                        break;
                    case Key_Layout.KeyLayoutType.Layout_3Row:
                        keyInfo = "30";
                        break;
                    default:
                        break;
                }
                #endregion
                string p_KeyCount = "KeyCount: " + keyInfo + newLine;
                
                string p_AccuracyHarshness = "AccuracyHarshness: " + noteEffector.accuracy.ToString() + newLine;
                string p_ApproachSpeed = "ApproachSpeed: " + noteEffector.approachSpeed.ToString() + newLine;
                #endregion

                #region Objects
                string objectsTag = "[Objects]";
                #endregion

                #region .rftm Information
                string[] rftmInformation = new string[]
                {
                   t_general +  
                   p_Author + 
                   p_AudioFileName +
                   p_Gamemode,

                   t_metadata + 
                   p_Title +
                   p_TitleUnicode +
                   p_Artist +
                   p_ArtistUnicode +
                   p_Creator +
                   p_ROFTID +
                   p_GROUPID,

                   t_difficulty +
                   p_StressBuild +
                   p_ObjectCount +
                   p_KeyCount +
                   p_AccuracyHarshness +
                   p_ApproachSpeed,

                   objectsTag
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

        rftmWriter.WriteLine(_data);
        rftmWriter.Close();
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
