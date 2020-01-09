using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool inSong = true;

    public static string[] accuracyString =
   {
        "Perfect",
        "Great",
        "Good",
        "Ok",
        "Miss"
    };

    public static int[] accuracyScore =
    {
        300,
        100,
        50,
        10,
        0
    };

    public static float[] stressAmount =
    {
       2f,
        0f,
        -1f,
        -2f,
        -5f
    };

    public static int sentScore;

    public static float sentStress;

    public static int consecutiveMisses;

    /*There's 3 modes:
     * STANDARD will be Keyboard and Mouse incorporated
     * KEY_ONLY will be what I've been having for ages
     * And TBR refers to "Type-By-Region"
     */
    public enum GameMode
    {
        STANDARD,
        TECHMEISTER,
        TBR_HOMEROW,
        TBR_ALL
    };
    [Header("Game Pause Overlay")]
    public bool isGamePaused = false;
    public GameObject PAUSE_OVERLAY;

    [Header("Edit Mode")]
    public bool editMode;

    [Header("Game Modes")]
    public GameMode gameMode;

    [Header("UI TEXT MESH PRO")]
    public TextMeshProUGUI TM_SONGNAME;
    public TextMeshProUGUI TM_TOTALNOTES;
    public TextMeshProUGUI TM_TOTALKEYS;
    public TextMeshProUGUI TM_SCORE;
    public TextMeshProUGUI TM_MAXSCORE;
    public TextMeshProUGUI TM_COMBO;
    public TextMeshProUGUI TM_COMBO_UNDERLAY;
    public TextMeshProUGUI TM_MAXCOMBO;
    public TextMeshProUGUI TM_DIFFICULTY;
    public TextMeshProUGUI TM_PERFECT;
    public TextMeshProUGUI TM_GREAT;
    public TextMeshProUGUI TM_GOOD;
    public TextMeshProUGUI TM_OK;
    public TextMeshProUGUI TM_MISS;
    public TextMeshProUGUI TM_ACCURACYPERCENTILE;
    public TextMeshProUGUI DEBUG_FILEDIR;

    [Header("UI IMAGES")]
    public Image IMG_STRESS;
    public Image IMG_STRESSBACKGROUND;
    public Image IMG_SCREEN_OVERLAY;
    public Image IMG_PROGRESSION_FILL;

    [Header("In-Game Statistics and Values")]
    public long totalScore;
    public long previousScore; //This will be used for a increasing effect
    public int combo;
    public int maxCombo;
    public float overallAccuracy = 100.00f; //The average accuracy during the song
    public float accuracyPercentile; //The data in which gets accuracy in percent;
    [Range(1f, 10f)] public float stressBuild = 5f;
    public int[] accuracyStats = new int[5];
    public bool isAutoPlaying;

    private readonly int reset = 0;

    int initialGain = 1;

    //Check for multiple input
    public static int multiInputValue;

    //RoftScouter will be our "Go collect some songs that I may or may not have
    //put in this directory.
    public RoftScouter scouter;

    //Time value
    float inputDelayTime = 0;
    float multiInputDuration = 0.1f;
    KeyCode[] inGameControlKeys = { KeyCode.Backspace, KeyCode.Escape };

    //Make to much easier to access other classes
    RoftPlayer roftPlayer;
    MapReader mapReader;
    MouseEvent mouse_env;
    KeyPress keypress_env;

    //Coroutine(s)
    IEnumerator crt_GameManagement;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
            Destroy(gameObject);
        #endregion
    }

    private void Start()
    {
        if (IMG_STRESS != null) IMG_STRESS.fillAmount = 0f;
        Application.targetFrameRate = 60;

        //Instanciate new scouter
        scouter = new RoftScouter();

        //Reference our roftPlayer
        /*I may be right or wrong, but this may be a flyweight pattern...
         * I think...
         * No... This is not... but it'll make things easy
         */
        roftPlayer = RoftPlayer.Instance;
        mapReader = MapReader.Instance;
        mouse_env = MouseEvent.Instance;
        keypress_env = KeyPress.Instance;

        //Assign coroutines
        crt_GameManagement = RUN_GAME_MANAGEMENT();
    }

    private void Update()
    {
        StartCoroutine(RUN_GAME_MANAGEMENT());
    }

    //GAME_MANAGEMENT Update
    IEnumerator RUN_GAME_MANAGEMENT()
    {
        if (inSong)
        {
            RunScoreSystem();
            RunUI();
            RunInGameControls();
            CheckSignsOfInput();
        }

        yield return null;
    }

    void RunScoreSystem()
    {
        //Raising effect
        if (previousScore < totalScore && !isGamePaused)
        {
            previousScore += combo ^ initialGain;
            initialGain += combo;
        }
        else
        {
            previousScore = totalScore;
            initialGain = reset + 1;
        }
    }

    void UpdateMaxCombo()
    {
        if (combo > maxCombo)
            maxCombo = combo;
    }

    void RunUI()
    {
        TM_SCORE.text = previousScore.ToString("D10");
        TM_COMBO.text = "x" + combo.ToString();
        TM_COMBO_UNDERLAY.text = "x" + combo.ToString();
        TM_DIFFICULTY.text = "DIFFICULTY: " + mapReader.difficultyRating.ToString("F2", CultureInfo.InvariantCulture);
        TM_MAXSCORE.text = "MAX SCORE:     " + mapReader.maxScore.ToString();

        //This will be temporary
        #region DEBUG_STATS_UI
        TM_PERFECT.text = "PERFECT:   " +
            accuracyStats[0].ToString() +
            " (" + Mathf.Floor((accuracyStats[0] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_GREAT.text = "GREAT:       " +
            accuracyStats[1].ToString() +
            " (" + Mathf.Floor((accuracyStats[1] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_GOOD.text = "GOOD:         " +
            accuracyStats[2].ToString() +
            " (" + Mathf.Floor((accuracyStats[2] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_OK.text = "OK:            " +
            accuracyStats[3].ToString() +
            " (" + Mathf.Floor((accuracyStats[3] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_MISS.text = "MISSES:       " +
            accuracyStats[4].ToString() +
            " (" + Mathf.Floor((accuracyStats[4] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_MAXCOMBO.text = "MAX COMBO:     "
            + maxCombo.ToString() +
            " (" + Mathf.Floor((maxCombo / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_ACCURACYPERCENTILE.text = "ACCURACY:     "
           + Mathf.Floor(overallAccuracy).ToString("F2", CultureInfo.InvariantCulture) + "%";
        #endregion

        //Be able to enable and disable Pause_Overlay
        PAUSE_OVERLAY.SetActive(isGamePaused);

        if (RoftPlayer.musicSource.isPlaying) ManageStressMeter();
    }

    void ManageStressMeter()
    {
        float stressBuildInPercent = stressBuild / 100;
        IMG_STRESS.fillAmount += ((stressBuildInPercent * (consecutiveMisses + (stressBuild / 10))) - sentStress) / 100;
        sentStress = reset;
    }

    void RunInGameControls()
    {
        if (Input.GetKeyDown(inGameControlKeys[0]))
            RestartSong();

        if (Input.GetKeyDown(inGameControlKeys[1]))
        {
            switch (isGamePaused)
            {
                case false:
                    if (RoftPlayer.musicSource.isPlaying)
                        Pause(); 
                    return;

                case true: 
                    UnPause(); 
                    return;
            }
        }
    }

    public void RestartSong()
    {
        NoteEffector.keyPosition = reset;

        for (int stat = 0; stat < accuracyStats.Length; stat++)
            accuracyStats[stat] = reset;

        combo = reset;
        maxCombo = reset;
        totalScore = reset;
        sentScore = reset;
        IMG_STRESS.fillAmount = reset;
        sentStress = reset;
        consecutiveMisses = reset;
        accuracyPercentile = reset;
        overallAccuracy = reset;
        RoftPlayer.musicSource.timeSamples = reset;
    }

    public void Pause()
    {
        //When we pause, we have to stop music, and turn isPaused to true
        roftPlayer.PauseMusic();
        isGamePaused = true;
        Time.timeScale = 0;
        return;
    }

    public void UnPause()
    {
        //Now we have to resume music, and turn isPaused to true
        roftPlayer.PlayMusic();
        isGamePaused = false;
        Time.timeScale = 1;
        return;
    }

    public void UpdateScore()
    {
        UpdateMaxCombo();
        initialGain++;
        totalScore += sentScore * combo;
    }

    public int GetSumOfStats()
    {
        //Do a calculation on accuracy stats
        int sumOfStats = 0;
        foreach (int value in accuracyStats)
        {
            sumOfStats += value;
        }
        return sumOfStats;
    }

    int CheckSignsOfInput()
    {
        if (gameMode == GameMode.TECHMEISTER)
        {
            multiInputValue = mouse_env.GetMouseInputValue() + keypress_env.GetKeyPressInputValue();

            //Check for second input
            if (multiInputValue > 0)
                StartMultiInputDelay();

            return multiInputValue;
        }
        return -1;
    }

    void StartMultiInputDelay()
    {
        inputDelayTime += Time.deltaTime;
        if (inputDelayTime > multiInputDuration)
            ResetMultiInputDelay();
    }

    public void ResetMultiInputDelay()
    {
        keypress_env.keyPressInput = reset;
        mouse_env.mouseMovementInput = reset;
        inputDelayTime = reset;
    }
}