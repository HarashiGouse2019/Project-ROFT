using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoftScouter
{
    public static RoftScouter Instance;
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

    public string curApp_Dir { get; set; } = Application.persistentDataPath + "/Songs";

    public DirectoryInfo directoryInfo { get; set; }

    //Construct ROFT_SCOUTER
    public RoftScouter()
    {
        //Get current application directory
        if (Instance != null)
            Instance = this;

        directoryInfo = new DirectoryInfo(curApp_Dir);

        List<FileInfo> obj = Commence_Scouting();
        foreach(FileInfo o in obj)
        {
            Debug.Log(o.Name);
        }
    }

    private List<FileInfo> Commence_Scouting()
    {
        List<FileInfo> discoveredSongFiles = new List<FileInfo>();
        //We want loop through each file, and extract every little information that 
        //we can graph to assign them to our Song_Entity list
        foreach(var discovered in directoryInfo.GetFiles("*.rftm", SearchOption.AllDirectories))
        {
            //Once a new entity's instansiated, we'll push to our discoveredSongFiles
            discoveredSongFiles.Add(discovered);
        }
        if (discoveredSongFiles.Count == 0)
            Debug.Log("New songs were not detected!!!");
        return discoveredSongFiles;
    }
}