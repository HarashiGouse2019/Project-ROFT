using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Song_Entity : MonoBehaviour
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

    public enum FC_STATUS
    {
        FC,
        APFC
    }

    //Song entity properties
    private string ent_SongName;
    private string ent_Artist;
    private RawImage ent_CoverArt;
    private uint ent_BPM;
    private float ent_InitialDifficulty;
    private long ent_TopScore;
    private long ent_TopCombo;
    private char ent_Grade;
    private bool ent_MarkAsFavorite;
    private FC_STATUS ent_FCStatus;

    public static Tuple<string, string, Image, uint, float, long, long, Tuple<char, bool, FC_STATUS>> ent_Song_Info { get; set; }

    //I wanna test somethign real quick
    List<long> numbers = new List<long>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
            AddNumber(UnityEngine.Random.Range(1, 100000000));

        Orderize();
    }

    void AddNumber(int value)
    {
        numbers.Add(value);
    }

    void Orderize()
    {
        numbers.Sort();
        numbers.Reverse();
    }
}
