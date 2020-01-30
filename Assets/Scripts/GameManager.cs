using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public static char[] accuracyGrade =
    {
        'S',
        'A',
        'B',
        'C',
        'D'
    };

    public static float[] gradePrecentageRequirement =
    {
        0.95f,
        0.90f,
        0.80f,
        0.70f,
        0
    };

    public static float[] stressAmount =
    {
       0.2f,
        0f,
        -0.1f,
        -0.2f,
        -0.3f
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
    [SerializeField] private bool isGamePaused = false;
    public bool IsGamePaused
    {
        get
        {
            return isGamePaused;
        }
    }

    [SerializeField] private GameObject PAUSE_OVERLAY = null;

    [Header("Game Modes")]
    [SerializeField] private GameMode gameMode = default;
    public GameMode GetGameMode
    {
        get
        {
            return gameMode;
        }
    }

    [Header("UI TEXT MESH PRO")]
    [SerializeField] private TextMeshProUGUI TM_SONGNAME = null;
    [SerializeField] private TextMeshProUGUI TM_TOTALNOTES = null;
    [SerializeField] private TextMeshProUGUI TM_TOTALKEYS = null;
    [SerializeField] private TextMeshProUGUI TM_SCORE = null;
    [SerializeField] private TextMeshProUGUI TM_MAXSCORE = null;
    [SerializeField] private TextMeshProUGUI TM_COMBO = null;
    [SerializeField] private TextMeshProUGUI TM_COMBO_UNDERLAY = null;
    [SerializeField] private TextMeshProUGUI TM_MAXCOMBO = null;
    [SerializeField] private TextMeshProUGUI TM_DIFFICULTY = null;
    [SerializeField] private TextMeshProUGUI TM_PERFECT = null;
    [SerializeField] private TextMeshProUGUI TM_GREAT = null;
    [SerializeField] private TextMeshProUGUI TM_GOOD = null;
    [SerializeField] private TextMeshProUGUI TM_OK = null;
    [SerializeField] private TextMeshProUGUI TM_MISS = null;
    [SerializeField] private TextMeshProUGUI TM_ACCURACYPERCENTILE = null;
    [SerializeField] private TextMeshProUGUI TM_ACCURACYGRADE = null;
    [SerializeField] private TextMeshProUGUI DEBUG_FILEDIR = null;

    [Header("UI IMAGES")]
    [SerializeField] private Image IMG_STRESS = null;
    [SerializeField] private Image IMG_STRESSBACKGROUND = null;
    [SerializeField] private Image IMG_SCREEN_OVERLAY = null;
    [SerializeField] private Image IMG_PROGRESSION_FILL = null;

    [Header("In-Game Statistics and Values")]
    private long totalScore;
    private long previousScore; //This will be used for a increasing effect
    private int Combo { set; get; }
    private int maxCombo;
    public float overallAccuracy = 100.00f; //The average accuracy during the song
    public float accuracyPercentile; //The data in which gets accuracy in percent;
    public float overallGrade = 0f;
    [Range(1f, 10f)] public float stressBuild = 5f;
    public int[] accuracyStats = new int[5];
    public bool isAutoPlaying;

    private readonly int reset = 0;

    private int initialGain = 1;

    //Time value
    private float inputDelayTime = 0;

    private readonly float multiInputDuration = 0.1f;

    //Check for multiple input
    public static int multiInputValue;

    [Header("In-Game Control Keys")]
    readonly KeyCode[] inGameControlKeys = {
        KeyCode.Backspace,
        KeyCode.Escape,
        KeyCode.Q,
        KeyCode.P,
        KeyCode.Semicolon,
        KeyCode.Comma
    };

    //RoftScouter will be our "Go collect some songs that I may or may not have
    //put in this directory.
    private RoftScouter scouter;

    //Make to much easier to access other classes
    private RoftPlayer roftPlayer;
    private MapReader mapReader;

    //This will be used for the GameManager to assure that
    //when we restart, all approach circles are inactive
    private readonly List<GameObject> activeApproachObjects = new List<GameObject>();

    //Countdown for player to prepare
    public bool isCountingDown = false;

    private delegate void Main();
    private Main core;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            CreateRecords();
            DontDestroyOnLoad(Instance);
        }
        else
            Destroy(gameObject);
        #endregion

        //Multicasting core delegate
        //When Invoked, it'll run all of these functions.
        //NEVER USE AN = SIGN WITH MUTLITASKING
        //It'll reset in reseting all castings that
        //it had in order to add a new one.
        #region Core Delegate MultiCasting
        core += RunScoreSystem;
        core += RunUI;
        core += RunInGameControls;
        core += CheckSignsOfInput;
        core += CheckStressMaxed;
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
    }

    private void Update()
    {
        if (RoftPlayer.Instance.record == false)
            StartCoroutine(RUN_GAME_MANAGEMENT());
    }

    //GAME_MANAGEMENT Update
    IEnumerator RUN_GAME_MANAGEMENT()
    {
        if (inSong && MapReader.KeysReaded)
            //Invoke core and all methods associated with it
            core.Invoke();

        yield return null;
    }

    void RunScoreSystem()
    {
        //Raising effect
        if (previousScore < totalScore && !isGamePaused)
        {
            previousScore += Combo ^ initialGain;
            initialGain += Combo;
        }
        else
        {
            previousScore = totalScore;
            initialGain = reset + 1;
        }
    }

    void UpdateMaxCombo()
    {
        if (Combo > maxCombo)
            maxCombo = Combo;
    }

    void RunUI()
    {
        TM_SCORE.text = previousScore.ToString("D10");
        TM_COMBO.text = "x" + Combo.ToString();
        TM_COMBO_UNDERLAY.text = "x" + Combo.ToString();
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
           + overallAccuracy.ToString("F2", CultureInfo.InvariantCulture) + "%";
        #endregion

        //Be able to enable and disable Pause_Overlay
        if (isCountingDown == false)
            PAUSE_OVERLAY.SetActive(isGamePaused);

        if (RoftPlayer.musicSource.isPlaying && SongProgression.isPassedFirstNote && !SongProgression.isFinished) ManageStressMeter();
    }

    void ManageStressMeter()
    {
        const uint millsInSec = 1000;
        float subtleProgression = (stressBuild / millsInSec);

        IMG_STRESS.fillAmount += (subtleProgression / (consecutiveMisses + stressBuild)) - sentStress;

        //Okay, so I have to think about this....
        //I think I have an idea for the Stress Build...
        /*I have two options for how this system should work
         * 
         * 1) Stress Build will be the multiplier for the sentStress, meaning I'll use
         *    sentStress * stressBuild
         *    
         *    They will definitely be added to the consecutiveMisses, because I want to get progressively worse
         *    as the player misses more.
         *    
         *    As for the subtle increase, stress progressiveness will be the stress build divided by
         *    the amount of milliseconds in a second (which is 1000)
         */
        sentStress = 0;
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
        mapReader.tapObjectReader.SequencePositionReset();
        mapReader.holdObjectReader.SequencePositionReset();
        mapReader.burstObjectReader.SequencePositionReset();

        for (int stat = 0; stat < accuracyStats.Length; stat++)
            accuracyStats[stat] = reset;

        //Reset all values
        Combo = reset;
        maxCombo = reset;
        totalScore = reset;
        sentScore = reset;
        IMG_STRESS.fillAmount = reset;
        sentStress = reset;
        consecutiveMisses = reset;
        accuracyPercentile = reset;
        overallAccuracy = reset;
        RoftPlayer.musicSource.timeSamples = reset;

        //Now we make sure all approach circles are inactive
        CollectApproachCircles();
        foreach (var activeCirlces in activeApproachObjects)
            activeCirlces.SetActive(false);

        //Now we clear our list.
        activeApproachObjects.Clear();

        SongProgression.isPassedFirstNote = false;
    }

    public void Pause()
    {
        //When we pause, we have to stop music, and turn isPaused to true
        if (RoftPlayer.musicSource.isPlaying)
        {
            roftPlayer.PauseMusic();
            isGamePaused = true;
            Time.timeScale = 0;
        }
        return;
    }

    public void UnPause()
    {
        //Now we give the player some time to prepare
        StartCoroutine(CountDown());
        return;
    }

    public void UpdateScore()
    {
        UpdateMaxCombo();
        UpdateGrade();
        initialGain++;
        totalScore += sentScore * Combo;
    }

    //Be sure that when our score updates we call this function
    //Sense I'm using a for loop, I only want to call it went needed
    //And that's when the player actually hits a note, and gets a score based
    //on accuracy.
    public void UpdateGrade()
    {
        /*Grade Calculations...
         * Grade S) Higher than or equal to 95%
         * Grade A) Higher than or equal to 90%
         * Grade B) Higher than or equal to 80%
         * Grade C) Higher than or equal to 70%
         * Grade D) Anything lower than 70%
         * 
         * This are accounted for Perfect Percentage
         * 
         * We want to find a way to use the accuracyGrade array
         * in terms of percentile (which seems intimidating).
         */

        for (int gradeIndex = 0; gradeIndex < gradePrecentageRequirement.Length; gradeIndex++)
        {
            //Check if overall Accuracy is above percentage values
            //We'll simple return out of for loop if statement is true
            if (overallAccuracy >= gradePrecentageRequirement[gradeIndex] * 100f)
            {
                TM_ACCURACYGRADE.text = "(" + accuracyGrade[gradeIndex].ToString() + ")";
                return;
            }
        }
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

    void CheckSignsOfInput()
    {
        if (gameMode == GameMode.TECHMEISTER)
        {
            multiInputValue = MouseEvent.Instance.GetMouseInputValue() + KeyPress.Instance.GetKeyPressInputValue();

            //Check for second input
            if (multiInputValue > 0)
                StartMultiInputDelay();
        }
    }

    void StartMultiInputDelay()
    {
        inputDelayTime += Time.deltaTime;
        if (inputDelayTime > multiInputDuration)
            ResetMultiInputDelay();
    }

    void CheckStressMaxed()
    {
        if (IMG_STRESS.fillAmount >= 0.99f)
            RestartSong();
    }

    void CollectApproachCircles()
    {
        GameObject[] discoveredObjs = GameObject.FindGameObjectsWithTag("approachCircle");
        foreach (var obj in discoveredObjs)
        {
            activeApproachObjects.Add(obj);
        }
    }

    public void ResetMultiInputDelay()
    {
        KeyPress.Instance.keyPressInput = reset;
        MouseEvent.Instance.mouseMovementInput = reset;
        inputDelayTime = reset;
    }

    public IEnumerator CountDown()
    {
        isGamePaused = false;
        isCountingDown = true;
        PAUSE_OVERLAY.SetActive(isGamePaused);
        for (int num = 3; num > reset; num--)
        {
            AudioManager.Instance.Play("Tick", _oneShot: true);
            yield return new WaitForSecondsRealtime(1f);
        }
        //Now we play music again, and stat
        roftPlayer.PlayMusic();

        //Update isGamePaused
        isGamePaused = !RoftPlayer.musicSource.isPlaying;

        //Set time scale back to 1, so everything should show motion over time
        Time.timeScale = 1;

        isCountingDown = false;

        yield return null;
    }

    public bool IsInteractable()
    {
        return (isGamePaused == false && isCountingDown == false);
    }

    void CreateRecords()
    {
        //This is when records.rft is nowhere to be seen
        string roftRecordPath = Application.persistentDataPath + @"/records.rft";
        if (!File.Exists(roftRecordPath))
        {
            File.Create(roftRecordPath);
            Debug.Log(roftRecordPath + " created.");
        }
    }

    public void SetCombo(int _value)
    {
        Combo = _value;
    }

    public void IncrementCombo()
    {
        Combo++;
    }

    #region Get Methods
    public Image GetSongProgressionFill()
    {
        return IMG_PROGRESSION_FILL;
    } 

    public TextMeshProUGUI GetTMCombo()
    {
        return TM_COMBO;
    }

    public TextMeshProUGUI GetTMComboUnderlay()
    {
        return TM_COMBO_UNDERLAY;
    }

    public Image GetScreenOverlay()
    {
        return IMG_SCREEN_OVERLAY;
    }
    #endregion
}