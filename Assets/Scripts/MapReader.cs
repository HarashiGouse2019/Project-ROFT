using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

using static ROFTIOMANAGEMENT.RoftIO;

public class MapReader : Singleton<MapReader>
{
    [SerializeField]
    private Canvas GameOverlayCanvas;

    [SerializeField] private static string m_name;

    [SerializeField] private static float difficultyRating;

    [SerializeField] private static float totalNotes;

    [SerializeField] private static float totalKeys;

    [SerializeField] private static long maxScore;

    public static int keyLayoutEnum;

    public static List<NoteObj> noteObjs = new List<NoteObj>();

    [Cakewalk.IoC.Dependency]
    public Key_Layout keyLayoutClass;

    //All Object Readers
    [Header("Object Readers")]
    [SerializeField] private TapObjectReader tapObjectReader;
    [SerializeField] private HoldObjectReader holdObjectReader;
    [SerializeField] private BurstObjectReader burstObjectReader;
    [SerializeField] private TrailObjectReader trailObjectReader;
    [SerializeField] private ClickObjectReader clickObjectReader;

    //I'll have to keep track of which files the MapReader is actually reading, or else I can't grab the information
    public static Song_Entity SongEntityBeingRead { get; set; }
    public static int DifficultyIndex { get; set; }

    readonly Thread readKeyThread;

    public static bool KeysReaded { get; private set; } = false;
    private static readonly object keyReadingLock = new object();

    public static void Read(int songEntityID, int difficultyValue)
    {


        /*After scouting, if songs has been found, go ahead and access
         this song and it's specified difficulty.*/

        if (!GameManager.SongsNotFound && !RoftPlayer.Record)
        {
            //Start scouting for songs one MapReader is initialized
            SongEntityBeingRead = MusicManager.GetSongEntity()[songEntityID];
            AssignRFTMNameToRead(SongEntityBeingRead, difficultyValue);
            return;
        }
        else if (GameManager.SongsNotFound)
        {
            Debug.Log("No songs had been detected.");
            Initialize();
            CalculateDifficultyRating();
            return;
        }
        else if (RoftPlayer.Record)
        {
            Initialize();
            CalculateDifficultyRating();
            return;
        }
    }

    static void Initialize()
    {
        if (Instance.keyLayoutClass != null)
            KeyLayoutAwake();
        else
        {
            Debug.LogError("Seems like KeyLayoutClass is null");
            return;
        }

        //Get other values such as Approach Speed, Stress Build, and Accuracy Harshness
        if (RoftPlayer.Instance != null && RoftPlayer.Record == false)
        {
            //Set the number where to find the difficulty tag
            int difficultyTag = InRFTMJumpTo("Difficulty", m_name);

            //Start assigning file data to following objects
            NoteEffector.Instance.ApproachSpeed = ReadPropertyFrom<float>(difficultyTag, "ApproachSpeed", m_name);
            GameManager.Instance.stressBuild = ReadPropertyFrom<float>(difficultyTag, "StressBuild", m_name);
            NoteEffector.Instance.Accuracy = ReadPropertyFrom<float>(difficultyTag, "AccuracyHarshness", m_name);

            //Set background image
            GameManager.Instance.SetInGameBackground(SongEntityBeingRead.BackgroundImage);

            //Calculate the max score
            maxScore = CalculateMaxScore();

            //Calculate difficulty rating
            CalculateDifficultyRating();
        }
        else
        {
            Debug.Log("For some reason, this is not being read....");
        }
    }

    static void ReadRFTMKeys(string _name)
    {
        lock (keyReadingLock)
        {
            string line;

            string rftmFilePath = _name;

            int maxKey = 0;

            #region Read .rftm data
            if (File.Exists(rftmFilePath))
            {
                //Read each line, split with a array string
                //Name it separator
                //Then use string.Split(separator, ...)
                const char separator = ',';
                int filePosition = 0;
                int targetPosition = InRFTMJumpTo("Objects", m_name);
                using (StreamReader rftmReader = new StreamReader(rftmFilePath))
                {
                    while (true)
                    {
                        line = rftmReader.ReadLine();
                        if (line == null)
                        {
                            KeysReaded = true;
                            return;
                        }


                        if (filePosition > targetPosition)
                        {
                            #region Parse and Convert Information
                            //We'll count the frequency of commas to determine
                            //that they are more values in the object
                            int countCommas = 0;
                            foreach (char c in line)
                                if (c == ',')
                                    countCommas++;

                            //We create a new key, and assign our data value to our key
                            NoteObj newNoteObj = new NoteObj();

                            newNoteObj.SetKeyID((uint)Convert.ToInt32(line.Split(separator)[0]));
                            newNoteObj.SetInitialSample(Convert.ToInt32(line.Split(separator)[1]));
                            newNoteObj.SetType((NoteObj.NoteObjType)Convert.ToInt32(line.Split(separator)[2]));

                            //Check for any miscellaneous values
                            if (countCommas > 2)
                                newNoteObj.miscellaneousValue1 = Convert.ToInt32(line.Split(separator)[3]);

                            else if (countCommas > 3)
                                newNoteObj.miscellaneousValue2 = Convert.ToInt32(line.Split(separator)[4]);
                            #endregion

                            /*This looks at the noteID
                             which is simply the number that is linked
                             to the keyID in the game.
                             */
                            if (newNoteObj.GetKey() > maxKey)
                            {
                                maxKey = (int)newNoteObj.GetKey();
                                totalKeys = maxKey + 1;
                            }

                            //Lastly, add our new key to the list
                            noteObjs.Add(newNoteObj);

                            //We'll be integrating our new Object Readers around this area.
                            //We'll distribute the basic values to each one.
                            #region Distribution to Readers
                            switch (newNoteObj.GetType())
                            {
                                case NoteObj.NoteObjType.Tap:
                                    DistributeTypeTo(Instance.tapObjectReader, newNoteObj);
                                    break;

                                case NoteObj.NoteObjType.Hold:
                                    DistributeTypeTo(Instance.holdObjectReader, newNoteObj);
                                    break;

                                case NoteObj.NoteObjType.Burst:
                                    DistributeTypeTo(Instance.burstObjectReader, newNoteObj);
                                    break;

                                default:
                                    break;
                            }
                            #endregion

                            //Update total Notes
                            totalNotes = noteObjs.Count;
                        }
                        filePosition++;
                    }
                }
            }
        }
        #endregion
    }

    static void CalculateDifficultyRating()
    {


        if (!RoftPlayer.Record)
        {
            RoftPlayer.LoadMusic();
            int totalNotes = noteObjs.Count;
            float songLengthInSec = RoftPlayer.musicSource.clip.length;
            float notesPerSec = (totalNotes / songLengthInSec);
            float totalKeys = Instance.keyLayoutClass.primaryBindedKeys.Count;
            float approachSpeedInPercent = (float)NoteEffector.Instance.ApproachSpeed / 100;
            float gameModeBoost = 0;
            const int maxKeys = 30;

            float calculatedRating = notesPerSec +
                (totalKeys / maxKeys) +
                approachSpeedInPercent +
                (RoftPlayer.musicSource.pitch / 2) +
                gameModeBoost;

            difficultyRating = calculatedRating;
        }
    }

    static long CalculateMaxScore()
    {
        long ini_Score = 0;
        for (long ini_Combo = 1; ini_Combo < totalNotes + 1; ini_Combo++)
        {
            ini_Score += 300 * ini_Combo;
        }
        return ini_Score;
    }

    static void KeyLayoutAwake()
    {
        Instance.GameOverlayCanvas.gameObject.SetActive(true);

        if (!Instance.keyLayoutClass.gameObject.activeInHierarchy)
        {
            Instance.keyLayoutClass.gameObject.SetActive(true);
        }

        if (!RoftPlayer.Record)
        {
            int difficultyTag = InRFTMJumpTo("Difficulty", m_name);
            string keyLayout = ReadPropertyFrom<string>(difficultyTag, "KeyLayout", m_name);
            Instance.keyLayoutClass.KeyLayout = (Key_Layout.KeyLayoutType)Enum.Parse(typeof(Key_Layout.KeyLayoutType), keyLayout);
            #region KeyCount through Enum
            switch (Instance.keyLayoutClass.KeyLayout)
            {
                case Key_Layout.KeyLayoutType.Layout_1x4:
                    totalKeys = 4;
                    break;
                case Key_Layout.KeyLayoutType.Layout_2x4:
                    totalKeys = 8;
                    break;
                case Key_Layout.KeyLayoutType.Layout_3x4:
                    totalKeys = 12;
                    break;
                case Key_Layout.KeyLayoutType.Layout_4x4:
                    totalKeys = 16;
                    break;
                case Key_Layout.KeyLayoutType.Layout_3x6:
                    totalKeys = 18;
                    break;
                case Key_Layout.KeyLayoutType.Layout_4x6:
                    totalKeys = 24;
                    break;
                case Key_Layout.KeyLayoutType.Layout_3x8:
                    totalKeys = 24;
                    break;
                case Key_Layout.KeyLayoutType.Layout_4x8:
                    totalKeys = 32;
                    break;
                default:
                    break;
            }
            #endregion
        }


        if (Key_Layout.Instance != null &&
            GameManager.Instance.GetGameMode == GameManager.GameMode.TECHMEISTER ||
            GameManager.Instance.GetGameMode == GameManager.GameMode.STANDARD)
            Instance.keyLayoutClass.SetUpLayout();

    }

    static void DistributeTypeTo(ObjectTypes _objectReader, NoteObj _key)
    {
        if (_objectReader != null)
            _objectReader.objects.Add(_key);
    }

    public static void AssignRFTMNameToRead(Song_Entity _song, int _difficultValue)
    {
        /*We want to be able to get retrieve a song entity, and wiht access to the song
         entity, we are able to read the .rftm (which are the difficulties) of the song.
         We will now call the ReadRFTM function to read the data that's been provided to us, thus
         ROFTPlayer can then retrieve information that is sorted on this object.

         That's at least what I hope this will do.*/

        int count = 0;
        foreach (Song_Entity.Song_Entity_Difficulty diffVal in _song.Difficulties)
        {
            if (count == _difficultValue)
            {
                SongEntityBeingRead = _song;
                DifficultyIndex = _difficultValue;
                m_name = diffVal.RFTMFile;
                ReadRFTMKeys(m_name);

                Initialize();
                CalculateDifficultyRating();
                return;
            }

            count++;
        }

        Debug.Log("An Error had occured... ");
        return;
    }

    #region Set Methods
    public void SetName(string _value)
    {
        m_name = _value;
    }
    #endregion

    #region Get Methods
    public static string GetName() => m_name;

    public static float GetDifficultyRating() => difficultyRating;

    public static float GetTotalNotes() => totalNotes;

    public static float GetTotalKeys() => totalKeys;

    public static long GetMaxScore() => maxScore;

    public static ObjectTypes GetReaderType<T>() where T : ObjectTypes
    {
        if (typeof(T) == Instance.tapObjectReader.GetType())
            return Instance.tapObjectReader;

        if (typeof(T) == Instance.holdObjectReader.GetType())
            return Instance.holdObjectReader;

        if (typeof(T) == Instance.burstObjectReader.GetType())
            return Instance.burstObjectReader;

        Debug.LogError("Failed to retrieve object reader...");
        return null;
    }

    #endregion
}
