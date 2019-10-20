using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Edit Mode")]
    public bool editMode;

    [Header("UI")]
    public TextMeshProUGUI TM_SCORE;
    public TextMeshProUGUI TM_COMBO;
    public TextMeshProUGUI TM_PERFECT;
    public TextMeshProUGUI TM_GREAT;
    public TextMeshProUGUI TM_GOOD;
    public TextMeshProUGUI TM_OK;
    public TextMeshProUGUI TM_MISS;

    public static string[] accuracyString =
    {
        "Perfect",
        "Great",
        "Good",
        "Ok",
        "CYKA!!!!"
    };

    public static int[] accuracyScore =
    {
        300,
        100,
        50,
        10,
        0
    };

    public static int sentScore;

    [Header("In-Game Statistics")]
    public int totalScore;
    public int combo;
    public int maxCombo;
    public int[] accuracyStats = new int[5];

    public List<GameObject> approachCircleTotal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
       approachCircleTotal =  new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        RunScoreSystem();
        RunUI();
    }

    void RunScoreSystem()
    {
        UpdateMaxCombo();
        totalScore += sentScore * combo;
        sentScore = 0;
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

        //This will be temporary
        TM_PERFECT.text = "PERFECT:   " + accuracyStats[0].ToString();
        TM_GREAT.text = "GREAT:     " + accuracyStats[1].ToString();
        TM_GOOD.text = "GOOD:      " + accuracyStats[2].ToString();
        TM_OK.text  = "OK:        " + accuracyStats[3].ToString();
        TM_MISS.text = "MISSES:    " + accuracyStats[4].ToString();
    }
}
