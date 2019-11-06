using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        1f,
        0f,
        -1f,
        -2f
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
        KEY_ONLY,
        TBR_HOMEROW,
        TBR_ALL
    };

    [Header("Edit Mode")]
    public bool editMode;

    [Header("Game Modes")]
    public GameMode gameMode;

    [Header("UI TEXT MESH PRO")]
    public TextMeshProUGUI TM_SONGNAME;
    public TextMeshProUGUI TM_TOTALNOTES;
    public TextMeshProUGUI TM_TOTALKEYS;
    public TextMeshProUGUI TM_SCORE;
    public TextMeshProUGUI TM_COMBO;
    public TextMeshProUGUI TM_MAXCOMBO;
    public TextMeshProUGUI TM_DIFFICULTY;
    public TextMeshProUGUI TM_PERFECT;
    public TextMeshProUGUI TM_GREAT;
    public TextMeshProUGUI TM_GOOD;
    public TextMeshProUGUI TM_OK;
    public TextMeshProUGUI TM_MISS;

    [Header("UI IMAGES")]
    public Image IMG_STRESS;
    public Image IMG_STRESSBACKGROUND;

    [Header("In-Game Statistics and Values")]
    public int totalScore;
    public int combo;
    public int maxCombo;
    [Range(1f, 10f)] public float stressBuild = 5f;
    public int[] accuracyStats = new int[5];
    public bool isAutoPlaying;

    private readonly int reset = 0;


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
        IMG_STRESS.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        #region Running Game Maintanence
        RunScoreSystem();
        RunUI();
        RunInGameControls(); 
        #endregion
    }

    void RunScoreSystem()
    {
        UpdateMaxCombo();
        totalScore += sentScore * combo;
        sentScore = reset;
    }

    void UpdateMaxCombo()
    {
        if (combo > maxCombo)
            maxCombo = combo;
    }

    void RunUI()
    {
        TM_SCORE.text = totalScore.ToString("D8");
        TM_COMBO.text = "x" + combo.ToString();
        TM_DIFFICULTY.text = "DIFFICULTY: " + MapReader.Instance.difficultyRating.ToString("F2", CultureInfo.InvariantCulture);

        //This will be temporary
        TM_PERFECT.text = "PERFECT:   " + 
            accuracyStats[0].ToString() + 
            " (" + Mathf.Floor((accuracyStats[0] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_GREAT.text = "GREAT:       " + 
            accuracyStats[1].ToString() + 
            " (" + Mathf.Floor((accuracyStats[1] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_GOOD.text = "GOOD:         " + 
            accuracyStats[2].ToString() + 
            " (" + Mathf.Floor((accuracyStats[2] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_OK.text  = "OK:            " + 
            accuracyStats[3].ToString() + 
            " (" + Mathf.Floor((accuracyStats[3] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_MISS.text = "MISSES:       " + 
            accuracyStats[4].ToString() + 
            " (" + Mathf.Floor((accuracyStats[4] / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture) + "%)";

        TM_MAXCOMBO.text = "MAX COMBO:     " 
            + maxCombo.ToString() + 
            " (" + Mathf.Floor((maxCombo / MapReader.Instance.totalNotes) * 100).ToString("F0", CultureInfo.InvariantCulture)  + "%)";

        if (EditorToolClass.musicSource.isPlaying) ManageStressMeter();
    }

    void ManageStressMeter()
    {
        float stressBuildInPercent = stressBuild / 100;
        IMG_STRESS.fillAmount += ((stressBuildInPercent * (consecutiveMisses + (stressBuild / 10))) - sentStress) / 100;
        sentStress = reset;
    }

    void RunInGameControls()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            RestartSong();
    }

    void RestartSong()
    {
        NoteEffect.Instance.keyPosition = reset;

        for (int stat = 0; stat < accuracyStats.Length; stat++)
            accuracyStats[stat] = reset;

        combo = reset;
        maxCombo = reset;
        totalScore = reset;
        IMG_STRESS.fillAmount = reset;
        EditorToolClass.musicSource.timeSamples = reset;
    }
}