using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public static bool ScoutingComplete { get; set; }

    private static AudioClip RequestedClip { get; set; }
    private static Sprite RequestedImage { get; set; }
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
        GameManager.Instance.StartCoroutine(ScoutingRoutine());
    }

    static IEnumerator ScoutingRoutine()
    {
        while (!ScoutingComplete)
        {
            //Target our defined directory to read information and data.
            DirectoryInfo = new DirectoryInfo(CurApp_Dir);

            /*We will then "Commence Scouting" by looking for all
            files with the .rftm extenstion in our CurApp_Dir variable.*/

            Func<List<FileInfo>> scoutingProcess = Commence_Scouting;

            List<FileInfo> obj = scoutingProcess.Invoke();

            /*With the files now collected, we read each individual file
            and convert them and their subfolders to SongEntities, which
            defines a songs and it's related difficulties.*/
            SongsFound = ConvertDiscoveredRFTMFilesToSongEntityObj(obj);

            /*We then assign information to our MusicManager will all the
            songs that's be found and converted.*/
            MusicManager.Songs = SongsFound;
            MusicManager.DisplaySongs(SongsFound);

            ScoutingComplete = true;

            yield return new WaitForEndOfFrame();
        }
    }

    private static List<FileInfo> Commence_Scouting()
    {
        List<FileInfo> discoveredSongFiles = new List<FileInfo>();

        /*From this point on, we will try to do the same operation as the foreach above,
        but this time, using the System.LING functions and stuff that it provides us...
        Not sure how all of this is going to work, but I'm willing to try. It kind of look very easy...*/

        //We get our array of files with the extension .rftm, and assign it to data.
        var data = DirectoryInfo.GetFiles("*rftm", SearchOption.AllDirectories);

        /*Then for all the files in the data, get all of them, and convert it to a list.
        By default, the expression is actually a Query, so by doing .ToList(),
        we run through our query immediately, and turn it to a List<T> type.*/
        discoveredSongFiles = (from files in data select files).ToList();

        //Have a bool detecting if there's no songs found.
        bool noSong = discoveredSongFiles.Count == 0;
        if (noSong == true)
        {
            Debug.Log("New songs were not detected!!!");
            GameManager.SongsNotFound = noSong;
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

        /*This will be used to compare the file's content, and 
        regularily create new SongEntities as needed.*/
        long currentGROUPID = 0;

        foreach (FileInfo fileNum in files)
        {
            int generalTag = InRFTMJumpTo("General", fileNum.FullName);
            int metadataTag = InRFTMJumpTo("Metadata", fileNum.FullName);
            int difficultyTag = InRFTMJumpTo("Difficulty", fileNum.FullName);
            int objectsTag = InRFTMJumpTo("Objects", fileNum.FullName);

            //Now with our tags assigned a value, I want to get all the information

            //In the [General] tag, we will retrieve the following...
            string audioFile = ReadPropertyFrom<string>(generalTag, "AudioFilename", fileNum.FullName);
            string backgroundImageFile = ReadPropertyFrom<string>(generalTag, "BackgroundImage", fileNum.FullName);

            //In the [Metadata] tag, we will retrieve the following...
            string songTitle = ReadPropertyFrom<string>(metadataTag, "Title", fileNum.FullName);
            string songArtist = ReadPropertyFrom<string>(metadataTag, "Artist", fileNum.FullName);
            int ROFTID = ReadPropertyFrom<int>(metadataTag, "ROFTID", fileNum.FullName);
            long GROUPID = ReadPropertyFrom<long>(metadataTag, "GROUPID", fileNum.FullName);

            //In the [Difficulty] tag, we will retrieve the following...
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
                //We instanstiate a new song entity
                Song_Entity newEntity = new Song_Entity();

                /*For one song in which holds its difficulty,
                we need to assign it a GROUPID,
                the title of the song...
                and the song artist.*/
                currentGROUPID = GROUPID;
                newEntity.SongTitle = songTitle;
                newEntity.SongArtist = songArtist;
                newEntity.GROUPID = GROUPID;

                /*We'll attempt to "Request" for the audio file defined in the .rftm file.
                The only way to do that is through the UnityEngine.Networking namespace
                through WebRequests.*/
                if (newEntity.AudioFile == null)
                {
                    while (true)
                    {
                        if (RequestingAudio(fileNum, audioFile))
                        {
                            newEntity.AudioFile = GetAudioClip();
                            break;
                        }
                    }
                }

                

                //Now that our SongEntity has been set, we'll add it to our list for further conversion.
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
    static Sprite GetRawImage() => RequestedImage;

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

    static bool RequestingImage(FileInfo _fileInfo, string _name)
    {
        string path = @"file:\\\" + _fileInfo.Directory + @"\" + _name;

        Debug.Log(path);

        Sprite image = ConvertTextureToSprite(LoadTexture(path), 100f, SpriteMeshType.Tight);

        RequestedImage = image;

        return true;
    }

    static Sprite ConvertTextureToSprite(Texture2D _texture, float _pixelPerUnit = 100.0f, SpriteMeshType _spriteType = SpriteMeshType.Tight)
    {
        //Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference

        Sprite newSprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0,0), _pixelPerUnit, 0, _spriteType);

        return newSprite;
    }

    static Texture2D LoadTexture(string _filePath)
    {
        /*We'll load a png or jpg file from disk to a Texture2D
         If we fail at doing so, return null.*/

        Texture2D tex2D;

        //We want to read the binary data of this file,
        //in order to know the formatting. With the formatting,
        //we'll use the data to create the image that we requested
        byte[] fileData;

        if (File.Exists(_filePath))
        {
            fileData = File.ReadAllBytes(_filePath);
            tex2D = new Texture2D(2, 2);
            if (tex2D.LoadImage(fileData))
                return tex2D;

        }

        return null;
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