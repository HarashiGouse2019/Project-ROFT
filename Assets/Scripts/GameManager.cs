using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Edit Mode")]
    public bool editMode;


    public static string[] accuracyString =
    {
        "Perfect",
        "Great",
        "Good",
        "Ok",
        "CYKA!!!!"
    };

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RunAccuracySystem()
    {
        //Accuracy is all depending on how harsh the song is
        //This will keep track how much
        //Perfect
        //Great
        //Good
        //Ok
        //Misses
        //That you got throughout the game play. That will also update the score system
        //as well.
    }

    void RunScoreSystem()
    {
        //Score is as follows:
        /*
         * Perfect is a score of 1000
         * Great is 1/2 of perfect (500)
         * Good is 1/4 of perfect (250)
         * Okay is 1/8 perfect (125)
         * And  miss is 0!!!!!
         * 
         * That is then multiplied by the chain of successful hits
         * that you got during gameplay. 
         * 
         * A buff is added from the difficuly, which will be songDif / 10 (So if songDif was 5, you get 0.5, or a 50% boost  from
         * initial scores)
         * 
         * Which mean a perfect would be 1000 / 0.5
         * in this case: Perfect score would be 2000
         * 
         * Scoring will be completely different with each song you play.
         * 
         * Difficulty is calculated by taking the number of keys ((4, 8, 10, 12, or 30 / totalAmount of notes) * 10
         * 
         * So let's check Perfume - Tokimeki  Lights with currently 873 notes
         * the lay out is a 4
         * 
         * (4 * 873) / 10
         */
    }
}
