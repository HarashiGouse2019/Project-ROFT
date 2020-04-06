using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using ROFTIOMANAGEMENT;

public class RoftCreator : MonoBehaviour
{
    /*RoftCreator will be the main script to handle creating new Beatmaps.
     Information like the name of the song, the artist of the song, metadata,
     difficult configurations, and sound will be creating through this script.
     
     This is the main process of creating Beatmaps until I can figure out how to
     create a good enough software editor to make creating Beatmaps a easier.
     */

    public static RoftCreator Instance;

    [Header("Audio Clip To Use")]
    [SerializeField] private AudioClip audioFile;

    [Header("Image/Video Background")]
    [SerializeField] private Texture backgroundImage;
    [SerializeField] private VideoClip backgroundVideo;

    [Header("General Setup")]
    [SerializeField] private string songTitle;
    [SerializeField] private string songArtist;
    [SerializeField] private string songTitleUnicode;
    [SerializeField] private string songArtistUnicode;
    [SerializeField] private string difficultyName;
    [SerializeField] private Key_Layout.KeyLayoutType keyLayout;

    [Header("Difficulty Setup")]
    [SerializeField, Range(1f, 10f)] private float approachSpeed;
    [SerializeField, Range(1f, 10f)] private float accuracyHarshness;
    [SerializeField, Range(1f, 10f)] private float stressBuild;

    [Header("Create New Difficulty")]
    [SerializeField, Tooltip("Toggle if creating new difficulty.")] private bool createNewDifficulty;
    [SerializeField, Tooltip("If creating a new difficulty, define what group the difficulty derives.")] private long GROUPID;

    //The filename that will be generated 
    public static string filename;

    //The audio file, background img, and video name and extension
    public static string audioFilePath;
    public static string backgroundFilePath;
    public static string videoFilePath;

    //Directory of the new song, so that notes can be recorded to it
    public static string newSongDirectoryPath;

    //Generated ROFTID 
    private int? ROFTID;

    //Song Identifier; Contains GROUPID-ROFTID
    private string songID;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        /*We'll check if we're recording.
         If we are, we generate a new .rftm file.
         
         If there's no defined GROUPID, we'll generate one,
         followed by a folder for all difficulties.*/

        if (!IDHIST.HistoryExists())
            IDHIST.NewHistory();

        if (RoftPlayer.Instance.record)
        {
            CheckForGROUPID();

            //audioExtension will be very important to take a song in a song folder, and converting it to a UnityAsset in which
            //we can assign to the RoftPlayer.cs
            string audioExt = null;// Path.GetExtension(AssetDatabase.GetAssetPath(audioFile));
            string backgroundImgExt = null;// Path.GetExtension(AssetDatabase.GetAssetPath(backgroundImage));

            audioFilePath = audioFile.name + audioExt;
            backgroundFilePath = backgroundImage.name + backgroundImgExt;

            //Filename will include the ID, Artis, SongName, and what Difficulty
            filename = songID + songArtist + " - " + songTitle + " (" + difficultyName + ")";

            //We then generate a new folder of the song
            newSongDirectoryPath = RoftIO.GenerateDirectory(GROUPID, songArtist, songTitle);

            //And create our roft file into it, as well as copy the AudioClip, Image, and Video that we used into this folder
            RoftIO.CreateNewRFTM(filename, newSongDirectoryPath + "/");

            try
            {
               //File.Copy(AssetDatabase.GetAssetPath(audioFile), newSongDirectoryPath + "/" + audioFile.name + audioExt);
                //File.Copy(AssetDatabase.GetAssetPath(backgroundImage), newSongDirectoryPath + "/" + backgroundImage.name + backgroundImgExt);
            }
            catch {/*This just means they already exists...*/};
        }
    }

    /// <summary>
    /// This is calculated by the beats per minute
    /// </summary>
    /// <returns></returns>
    IEnumerator Tick(float _bpm)
    {
        while (true)
        {
            AudioManager.Play("Tick");
            yield return new WaitForSeconds(60f / _bpm);
           

        }
    }

    /// <summary>
    /// Check if the user wants to create a new song, or a difficulty.
    /// </summary>
    void CheckForGROUPID()
    {
        if (GROUPID == 0 && createNewDifficulty == false)
        {
            GROUPID = RoftIO.GenerateGROUPID();
            ROFTID = RoftIO.GenerateROFTID(GROUPID);
        }
        else
        {
            //Since GROUPID is defined in the inspector, we don't need to generate one
            //We just need to generate a ROFTID with the specfied GROUPID
            ROFTID = RoftIO.GenerateROFTID(GROUPID);
        }
        songID = "(" + GROUPID + "-" + ROFTID + ") ";
        IDHIST.Write(songID);
    }

    #region Get Methods
    /// <summary>
    /// Get audio file being used.
    /// </summary>
    /// <returns>AudioClip</returns>
    public AudioClip GetAudioFile() => audioFile;

    /// <summary>
    /// Get background image being used.
    /// </summary>
    /// <returns>RawImage</returns>
    public Texture GetBackgroundImage() => backgroundImage;

    /// <summary>
    /// Get background video being used.
    /// </summary>
    /// <returns>VideoClip</returns>
    public  VideoClip GetVideoClip() => backgroundVideo;

    /// <summary>
    /// Get song title being used.
    /// </summary>
    /// <param name="_inUnicode">Return the unicode of song title</param>
    /// <returns>String</returns>
    public string GetSongTitle(bool _inUnicode = false) => (_inUnicode == true) ? songTitleUnicode : songTitle;

    /// <summary>
    /// Get song artist being used.
    /// </summary>
    /// <param name="_inUnicode">Return the unicode of song artist</param>
    /// <returns></returns>
    public string GetSongArtist(bool _inUnicode = false) => (_inUnicode == true) ? songArtistUnicode : songArtist;

    /// <summary>
    /// Get difficulty name being used.
    /// </summary>
    /// <returns></returns>
    public  string GetDifficultyName() => difficultyName;

    /// <summary>
    /// Get total keys being used.
    /// </summary>
    /// <returns></returns>
    public  Key_Layout.KeyLayoutType GetTotalKeys() => keyLayout;

    /// <summary>
    /// Get the harshness of difficulty being used.
    /// </summary>
    /// <returns></returns>
    public float GetAccuracyHarshness() => accuracyHarshness;

    /// <summary>
    /// Get the speed of circle enclosing.
    /// </summary>
    /// <returns></returns>
    public float GetApproachSpeed() => approachSpeed;

    /// <summary>
    /// Get how much stress will build up.
    /// </summary>
    /// <returns></returns>
    public float GetStressBuild() => stressBuild;

    /// <summary>
    /// Get Group ID.
    /// </summary>
    /// <returns></returns>
    public string GetGROUPID() => GROUPID.ToString();

    /// <summary>
    /// Get Roft ID.
    /// </summary>
    /// <returns></returns>
    public string GetROFTID() => ROFTID.ToString();

    /// <summary>
    /// Get Key Layout being used.
    /// </summary>
    /// <returns></returns>
    public Key_Layout.KeyLayoutType GetKeyLayout() => keyLayout;

    #endregion
}
