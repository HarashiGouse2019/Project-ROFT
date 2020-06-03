using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankSystem : MonoBehaviour
{
    private static RankSystem Instance;
    /*Rank System will be responsible for reading the score of the player, and determining
     what rank they get based on song configuration*/

    //Array of Rank Lettering.
    //Having it like this will also allow easy customization
    [SerializeField]
    private Sprite[] letterSprites = new Sprite[7];

    [SerializeField]
    private Image letterRankImg;

    //An enumerator that states our grade
    public enum Grade
    {
        SSS,
        SS,
        S,
        A,
        B,
        C,
        D
    }

    //Grade Percent Requirement
    public static float[] GPR =
    {
        1f,
        0.98f,
        0.95f,
        0.90f,
        0.80f,
        0.70f,
        0
    };

    //Grade variable
    Grade gradeRank = Grade.SSS;

    void Awake()
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

    //Be sure that when our score updates we call this function
    //Sense I'm using a for loop, I only want to call it went needed
    //And that's when the player actually hits a note, and gets a score based
    //on accuracy.
    public static void UpdateGrade()
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

        for (int gradeIndex = 0; gradeIndex < GPR.Length; gradeIndex++)
        {
            //Check if overall Accuracy is above percentage values
            //We'll simple return out of for loop if statement is true
            if (GameManager.Instance.overallAccuracy >= GPR[gradeIndex] * 100f)
            {
                //This will give use string value of our Grade enumerator
                Instance.gradeRank = (Grade)gradeIndex;
                Instance.letterRankImg.sprite = Instance.letterSprites[gradeIndex];
                return;
            }
        }
    }
}
