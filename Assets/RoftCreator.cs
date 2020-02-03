using System.Collections;
using System.Collections.Generic;
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

    [Header("Audio Clip To Use")]
    [SerializeField] private static AudioClip audioFile;

    [Header("Image/Video Background")]
    [SerializeField] private static RawImage backgroundImage;
    [SerializeField] private static VideoClip backgroundVideo;

    [Header("General Setup")]
    [SerializeField] private static string songTitle;
    [SerializeField] private static string songArtist;
    [SerializeField] private static string songTitleUnicode;
    [SerializeField] private static string songArtistUnicode;
    [SerializeField] private static string difficultyName;
    [SerializeField] private static Key_Layout.KeyLayoutType keyLayout;

    [Header("Difficulty Setup")]
    [SerializeField, Range(1f, 10f)] private static float approachSpeed;
    [SerializeField, Range(1f, 10f)] private static float accuracyHarshness;
    [SerializeField, Range(1f, 10f)] private static float stressBuild;

    [Header("Create New Difficulty")]
    [SerializeField, Tooltip("Toggle if creating new difficulty.")] private static bool createNewDifficulty;
    [SerializeField, Tooltip("If creating a new difficulty, define what group the difficulty derives.")] private static int GROUPID;

    private void Awake()
    {
        /*We'll check if we're recording.
         If we are, we generate a new .rftm file.
         
         If there's no defined GROUPID, we'll generate one,
         followed by a folder for all difficulties.*/

    }

    #region Get Methods
    /// <summary>
    /// Get audio file being used.
    /// </summary>
    /// <returns>AudioClip</returns>
    public static AudioClip GetAudioFile()
    {
        return audioFile;
    } 

    /// <summary>
    /// Get background image being used.
    /// </summary>
    /// <returns>RawImage</returns>
    public static RawImage GetBackgroundImage()
    {
        return backgroundImage;
    }

    /// <summary>
    /// Get background video being used.
    /// </summary>
    /// <returns>VideoClip</returns>
    public static VideoClip GetVideoClip()
    {
        return backgroundVideo;
    }

    /// <summary>
    /// Get song title being used.
    /// </summary>
    /// <param name="_inUnicode">Return the unicode of song title</param>
    /// <returns>String</returns>
    public static string GetSongTitle(bool _inUnicode = false)
    {
        switch (_inUnicode)
        {
            case false:
                return songTitle;
            case true:
                return songTitleUnicode;
        }
    }

    /// <summary>
    /// Get song artist being used.
    /// </summary>
    /// <param name="_inUnicode">Return the unicode of song artist</param>
    /// <returns></returns>
    public static string GetSongArtist(bool _inUnicode = false)
    {
        switch (_inUnicode)
        {
            case false:
                return songArtist;
            case true:
                return songArtistUnicode;
        }
    }

    /// <summary>
    /// Get difficulty name being used.
    /// </summary>
    /// <returns></returns>
    public static string GetDifficultyName()
    {
        return difficultyName;
    }

    /// <summary>
    /// Get total keys being used.
    /// </summary>
    /// <returns></returns>
    public static Key_Layout.KeyLayoutType GetTotalKeys()
    {
        return keyLayout;
    }

    /// <summary>
    /// Get the harshness of difficulty being used.
    /// </summary>
    /// <returns></returns>
    public static float GetAccuracyHarshness()
    {
        return accuracyHarshness;
    }

    /// <summary>
    /// Get the speed of circle enclosing.
    /// </summary>
    /// <returns></returns>
    public static float GetApproachSpeed()
    {
        return approachSpeed;
    }

    /// <summary>
    /// Get how much stress will build up.
    /// </summary>
    /// <returns></returns>
    public static float GetStressBuild()
    {
        return stressBuild;
    }

    /// <summary>
    /// Get Group ID.
    /// </summary>
    /// <returns></returns>
    public static string GetGROUDIP()
    {
        return GROUPID.ToString();
    }
    #endregion


}
