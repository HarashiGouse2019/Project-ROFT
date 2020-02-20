using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using static ROFTIOMANAGEMENT.RoftIO;

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

    public List<Song_Entity> SongsFound { get; set; } = new List<Song_Entity>();

    //Construct ROFT_SCOUTER
    public RoftScouter()
    {
        //Get current application directory
        if (Instance != null)
            Instance = this;

        directoryInfo = new DirectoryInfo(curApp_Dir);

        List<FileInfo> obj = Commence_Scouting();
        SongsFound = ConvertDiscoveredRFTMFilesToSongEntityObj(obj);
        MusicManager.Instance.songs = SongsFound;
    }

    private List<FileInfo> Commence_Scouting()
    {
        List<FileInfo> discoveredSongFiles = new List<FileInfo>();
        //We want loop through each file, and extract every little information that 
        //we can graph to assign them to our Song_Entity list
        foreach (var discovered in directoryInfo.GetFiles("*.rftm", SearchOption.AllDirectories))
        {
            //Once a new entity's instansiated, we'll push to our discoveredSongFiles
            discoveredSongFiles.Add(discovered);
        }
        if (discoveredSongFiles.Count == 0)
            Debug.Log("New songs were not detected!!!");
        return discoveredSongFiles;
    }

    public List<Song_Entity> ConvertDiscoveredRFTMFilesToSongEntityObj(List<FileInfo> files)
    {
        /*Our next greatest challenge is right here...
         Taking our generated information, and turning them into actual gameObjects, that we can used for the game
         
         There's already a place sort of involved, and it has to do with how I was able to find all the .rftm files in their respective
         folders.
         
         This operation may require a nested loop; one for the GROUPID, then for the ROFTID. In the second loop,
         this is when we instantiate a new Song_Entity class, and read into the .rftm files. This is where we can use
         the RoftIO functions I created.

        I need a variable that can keep track if I'm dealing with a new GROUPID
         */
        List<Song_Entity> convertedObj = new List<Song_Entity>();

        //This will be used to compare the file's content, and 
        //regularily create new SongEntities as needed.
        long currentGROUPID = 0;

        int loopAmount = 0;

        foreach (FileInfo f in files)
        {
            int generalTag = InRFTMJumpTo("General", f.FullName);
            int metadataTag = InRFTMJumpTo("Metadata", f.FullName);
            int difficultyTag = InRFTMJumpTo("Difficulty", f.FullName);

            //Now with our tags assigned a value, I want to get all the information
            string audioFile = ReadPropertyFrom<string>(generalTag, "AudioFilename", f.FullName);

            string songTitle = ReadPropertyFrom<string>(metadataTag, "Title", f.FullName);
            string songArtist = ReadPropertyFrom<string>(metadataTag, "Artist", f.FullName);
            int ROFTID = ReadPropertyFrom<int>(metadataTag, "ROFTID", f.FullName);
            long GROUPID = ReadPropertyFrom<long>(metadataTag, "GROUPID", f.FullName);

            string difficultyName = ReadPropertyFrom<string>(difficultyTag, "DifficultyName", f.FullName);
            float stressBuild = ReadPropertyFrom<float>(difficultyTag, "StressBuild", f.FullName);
            int keyCount = ReadPropertyFrom<int>(difficultyTag, "KeyCount", f.FullName);
            float accuracyHarshness = ReadPropertyFrom<float>(difficultyTag, "AccuracyHarshness", f.FullName);
            float approachSpeed = ReadPropertyFrom<float>(difficultyTag, "ApproachSpeed", f.FullName);

            //And then I want to know how many objects this song has
            int totalNotes = GetNoteObjectCountInRFTMFile(f.FullName);

            //Now we can instantiate a new object
            if (!CompareGroupID(ref currentGROUPID, ref GROUPID))
            {
                Song_Entity newEntity = new Song_Entity();
                currentGROUPID = GROUPID;
                newEntity.SongTitle = songTitle;
                newEntity.SongArtist = songArtist;
                newEntity.GROUPID = GROUPID;
                convertedObj.Add(newEntity);

                //We generate a difficulty once number change because if we don't, it'll skip any ROFTID with 100
                GenerateDifficulty(ROFTID, difficultyName, approachSpeed, stressBuild, accuracyHarshness, totalNotes, keyCount, f, convertedObj);
            }
            else
                //Now we generate the rest of the difficulties in the folder.
                GenerateDifficulty(ROFTID, difficultyName, approachSpeed, stressBuild, accuracyHarshness, totalNotes, keyCount, f, convertedObj);

        }

        return convertedObj;
    }

    public bool CompareGroupID(ref long _val1, ref long _val2) => (_val1 == _val2);

    public void GenerateDifficulty(int _ROFTID, string _difficultyName, float _approachSpeed, float _stressBuild, float _accuracyHarshness, int _totalNotes, int _keyCount, FileInfo _fileInfo, List<Song_Entity> _convertedObj)
    {
        Song_Entity.Song_Entity_Difficulty newDifficulty = new Song_Entity.Song_Entity_Difficulty();
        newDifficulty.ROFTID = _ROFTID;
        newDifficulty.DifficultyName = _difficultyName;
        newDifficulty.ApproachSpeed = _approachSpeed;
        newDifficulty.StressBuild = _stressBuild;
        newDifficulty.Accuracy = _accuracyHarshness;
        newDifficulty.TotalNotes = _totalNotes;
        newDifficulty.TotalKeys = _keyCount;
        newDifficulty.RFTMFile = _fileInfo.FullName;
        _convertedObj[_convertedObj.Count - 1].AddNewDifficulty(newDifficulty);
    }

}