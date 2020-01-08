using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roft_Scouter
{
    public static Roft_Scouter Instance;
    #region Roft_Scouter Outline/Plan
    /*Roft_Scouter is responsible for the detection of a new
    * file in the "Songs" direction in Project-Roft.
    * 
    * In the game, you can "Commence Scouting" to see if there was any
    * changes done to the "Songs" directory
    * 
    * The song list will then get updated, and it'll notify you.
    * 
    * You can also do a Scouting simply with F5
    */
    #endregion

    public DirectoryInfo directoryInfo;

    //Construct ROFT_SCOUTER
    public Roft_Scouter()
    {
        //Get current application directory
        if (Instance != null)
            Instance = this;

        string curApp_Dir = Application.persistentDataPath;
        GameManager.Instance.DEBUG_FILEDIR.text = curApp_Dir;
    }

    private List<Song_Entity> Commence_Scouting()
    {
        List<Song_Entity> discoveredSongFiles = new List<Song_Entity>();
        //We want loop through each file, and extract every little information that 
        //we can graph to assign them to our Song_Entity list
        foreach(var discovered in directoryInfo.GetFiles("*.rftm"))
        {

        }

        return discoveredSongFiles;
    }
}