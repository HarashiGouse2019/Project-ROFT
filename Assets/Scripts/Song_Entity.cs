using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Song_Entity
{
    #region Outline/Plan
    //Song_Entity class will take up the following values:

    /*Name
     * Artist
     * BPM
     * Initial Difficulty (which is the overall average of avaliable difficulties)
     * Cover Art
     * Top Score (all scores, but top is on display)
     * Top Combo (from all scores, but will be on top)
     * Grade
     * Marked as favorite
     * FC Status
     * 
     * In-Game songs initially have 3 difficulties
     * and those difficulty are given a rating of 10
     * 
     * with 3 difficulties with a max rating of 10, those are
     * calculated for the inital difficulty, which only has
     * 3 stars total
     * 
     * Initial Difficulty of a song is equal to the
     * sum of all 3 rating difficults in a song
     * minus 10.
     * Take the difference and divide by 3.
     * 
     * Then you round that number to get inital difficulty
     * 
     * For example: 
     * Tokimeki Lights
     *  Easy - 3.60
     *  Normal - 4.35
     *  Hard - 5.09
     *  
     *  Sum of All 3 - 13.04
     *  
     *  13.04 becomes 3.04
     *  
     *  3.04 becomes 1.013...
     *  
     *  Initial Difficulty - 1 Star(s)
     *  
     * Yooh - Destroy
     *  Easy - 4.68
     *  Normal - 5.48
     *  Hard - 6.24
     *  
     *  Sum of All 3 - 16.04
     *  
     *  16.04 becomes 6.04
     *  
     *  6.04 becomes 2.13...
     *  
     *  Initial Difficulty - 2 Star(s)
     * 
     * FC can be achieved no matter the accuracy, however
     * APFC (All-Perfect Full-Combo) is achieved with 100% perfect
     */
    #endregion
    [System.Serializable]
    public class Song_Entity_Difficulty
    {
        //Song Difficulty will have a lot of information in it, so yeah.
        public enum FC_STATUS
        {
            FC,
            APFC
        }
        public int ROFTID ;
        public string DifficultyName ;
        public float DifficultyRating ;
        public float ApproachSpeed ;
        public float StressBuild ;
        public float Accuracy ;
        public float TotalNotes ;
        public float TotalKeys ;
        public long PossibleMaxScore ;
        public FC_STATUS FullComboStatus ;

        //We'll need to stat the file that the difficulty is, so we can read it
        public string RFTMFile ;


    }
    public string SongTitle ;
    public string SongArtist ;
    public long GROUPID ;
    public float InitialDifficultyRating ;
    public AudioClip AudioFile ;
    public Texture2D BackgroundImage ;
    public List<Song_Entity_Difficulty> Difficulties = new List<Song_Entity_Difficulty>();

    public void AddNewDifficulty(Song_Entity_Difficulty _difficulty)
    {
        Difficulties.Add(_difficulty);
    }
}
