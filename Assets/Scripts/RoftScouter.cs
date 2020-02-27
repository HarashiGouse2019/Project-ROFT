using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;

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

    public enum MediaType
    {
        AUDIO,
        RAWIMAGE,
        VIDEO
    }

    public static string CurApp_Dir { get; set; } = Application.persistentDataPath + "/Songs";

    public static DirectoryInfo DirectoryInfo { get; set; }

    public static List<Song_Entity> SongsFound { get; set; } = new List<Song_Entity>();

    private static AudioClip RequestedClip { get; set; }
    private static Texture2D RequestedImage { get; set; }
    private static VideoClip RequestedVideo { get; set; }

    public RoftScouter()
    {
        //Get current application directory
        if (Instance != null)
            Instance = this;
    }

    //OnStart Event
    public static void OnStart()
    {
        DirectoryInfo = new DirectoryInfo(CurApp_Dir);

        List<FileInfo> obj = Commence_Scouting();
        SongsFound = ConvertDiscoveredRFTMFilesToSongEntityObj(obj);
        MusicManager.s_songs = SongsFound;
        MusicManager.Instance.songs = MusicManager.s_songs;
    }

    private static List<FileInfo> Commence_Scouting()
    {
        List<FileInfo> discoveredSongFiles = new List<FileInfo>();
        //We want loop through each file, and extract every little information that 
        //we can graph to assign them to our Song_Entity list
        foreach (var discovered in DirectoryInfo.GetFiles("*.rftm", SearchOption.AllDirectories))
        {
            //Once a new entity's instansiated, we'll push to our discoveredSongFiles
            discoveredSongFiles.Add(discovered);
        }

        if (discoveredSongFiles.Count == 0)
        {
            Debug.Log("New songs were not detected!!!");
            GameManager.SongsNotFound = true;
        }

        return discoveredSongFiles;
    }

    public static List<Song_Entity> ConvertDiscoveredRFTMFilesToSongEntityObj(List<FileInfo> files)
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

        foreach (FileInfo fileNum in files)
        {
            int generalTag = InRFTMJumpTo("General", fileNum.FullName);
            int metadataTag = InRFTMJumpTo("Metadata", fileNum.FullName);
            int difficultyTag = InRFTMJumpTo("Difficulty", fileNum.FullName);
            int objectsTag = InRFTMJumpTo("Objects", fileNum.FullName);

            //Now with our tags assigned a value, I want to get all the information
            string audioFile = ReadPropertyFrom<string>(generalTag, "AudioFilename", fileNum.FullName);
            string backgroundImageFile = ReadPropertyFrom<string>(generalTag, "BackgroundImage", fileNum.FullName);

            string songTitle = ReadPropertyFrom<string>(metadataTag, "Title", fileNum.FullName);
            string songArtist = ReadPropertyFrom<string>(metadataTag, "Artist", fileNum.FullName);
            int ROFTID = ReadPropertyFrom<int>(metadataTag, "ROFTID", fileNum.FullName);
            long GROUPID = ReadPropertyFrom<long>(metadataTag, "GROUPID", fileNum.FullName);

            string difficultyName = ReadPropertyFrom<string>(difficultyTag, "DifficultyName", fileNum.FullName);
            float stressBuild = ReadPropertyFrom<float>(difficultyTag, "StressBuild", fileNum.FullName);
            int keyCount = ReadPropertyFrom<int>(difficultyTag, "KeyCount", fileNum.FullName);
            float accuracyHarshness = ReadPropertyFrom<float>(difficultyTag, "AccuracyHarshness", fileNum.FullName);
            float approachSpeed = ReadPropertyFrom<float>(difficultyTag, "ApproachSpeed", fileNum.FullName);

            //And then I want to know how many objects this song has
            int totalNotes = GetNoteObjectCountInRFTMFile(fileNum.FullName);

            //Now we can instantiate a new object
            if (!CompareGroupID(ref currentGROUPID, ref GROUPID))
            {
                Song_Entity newEntity = new Song_Entity();

                currentGROUPID = GROUPID;
                newEntity.SongTitle = songTitle;
                newEntity.SongArtist = songArtist;
                newEntity.GROUPID = GROUPID;

                //Request for audio
                if (newEntity.AudioFile == null)
                {
                    while (true)
                    {
                        if (RequestingAudio(fileNum, audioFile) && RequestingImage(fileNum, backgroundImageFile))
                        {
                            newEntity.AudioFile = GetAudioClip();
                            newEntity.BackgroundImage = GetRawImage();
                            break;
                        }
                    }
                }

                convertedObj.Add(newEntity);

                //Generate our first difficulty
                GenerateDifficulty(ROFTID, difficultyName, approachSpeed, stressBuild, accuracyHarshness, totalNotes, keyCount, fileNum, convertedObj);
            }
            else
                //Now we generate the rest of the difficulties in the folder.
                GenerateDifficulty(ROFTID, difficultyName, approachSpeed, stressBuild, accuracyHarshness, totalNotes, keyCount, fileNum, convertedObj);

        }

        return convertedObj;
    }

    static AudioClip GetAudioClip() => RequestedClip;
    static Texture2D GetRawImage() => RequestedImage;

    static bool RequestingAudio(FileInfo _url, string _name)
    {
        string link = @"file:\\\" + _url.Directory + @"\" + _name;
        #region Requesting Audio
        using (UnityWebRequest requestAudio = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
        {
            UnityWebRequestAsyncOperation operation = requestAudio.SendWebRequest();

            while (!operation.isDone)
                continue;

            if (requestAudio.isNetworkError || requestAudio.isHttpError)
            {
                Debug.Log("Failed to load audio.");
                return operation.isDone;
            }
            else
            {
                RequestedClip = DownloadHandlerAudioClip.GetContent(requestAudio);
                RequestedClip.name = _name;
                return operation.isDone;
            }
        }
        #endregion
    }

    static bool RequestingImage(FileInfo _url, string _name)
    {
        string link = @"file:\\\" + _url.Directory + @"\" + _name;
        return true;
        //#region Requesting Image
        //using (UnityWebRequest requestImage = UnityWebRequestTexture.GetTexture(link))
        //{
        //    UnityWebRequestAsyncOperation operation = requestImage.SendWebRequest();
            
        //    while (!operation.isDone)
        //        continue;


        //    if (requestImage.isNetworkError || requestImage.isHttpError)
        //    {
        //        Debug.Log("Failed to load image.");
        //        return operation.isDone;
        //    }
        //    else
        //    {
        //        RequestedImage = DownloadHandlerTexture.GetContent(requestImage);
        //        RequestedImage.name = _name;
        //        return operation.isDone;
        //    }
        //}
        //#endregion
    }

    static bool CompareGroupID(ref long _val1, ref long _val2) => (_val1 == _val2);

    static void GenerateDifficulty(int _ROFTID, string _difficultyName, float _approachSpeed, float _stressBuild, float _accuracyHarshness, int _totalNotes, int _keyCount, FileInfo _fileInfo, List<Song_Entity> _convertedObj)
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