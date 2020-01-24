using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    public static MapReader Instance;

    public string m_name;

    public float difficultyRating;

    public float totalNotes;

    public float totalKeys;

    public long maxScore;

    public int keyLayoutEnum;

    public List<NoteObj> noteObjs = new List<NoteObj>();

    public Key_Layout keyLayoutClass = Key_Layout.Instance;

    //All Object Readers
    [Header("Object Readers")]
    public TapObjectReader tapObjectReader;
    public HoldObjectReader holdObjectReader;
    public SlideObjectReader slideObjectReader;
    public TrailObjectReader trailObjectReader;
    public ClickObjectReader clickObjectReader;

    private RoftIO mainIO;
    private readonly object noteReadLock = new object();

    private void Awake()
    {
        mainIO = GameManager.mainIO;
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

    }

    private void Start()
    {

        ReadRFTMKeys(m_name);

        if (keyLayoutClass != null)
            KeyLayoutAwake();

        //Get other values such as Approach Speed, Stress Build, and Accuracy Harshness
        if (RoftPlayer.Instance != null && !RoftPlayer.Instance.record)
        {
            int difficultyTag = GameManager.mainIO.InRFTMJumpTo("Difficulty", m_name);
            NoteEffector.Instance.approachSpeed = mainIO.ReadPropertyFrom<float>(difficultyTag, "ApproachSpeed", m_name);
            GameManager.Instance.stressBuild = mainIO.ReadPropertyFrom<float>(difficultyTag, "StressBuild", m_name);
            NoteEffector.Instance.accuracy = mainIO.ReadPropertyFrom<float>(difficultyTag, "AccuracyHarshness", m_name);
            maxScore = CalculateMaxScore();
        }
        else
        {
            Debug.Log("For some reason, this is not being read....");
        }

    }

    private void Update()
    {
        if (GameManager.inSong == true)
            difficultyRating = CalculateDifficultyRating();
    }

    void ReadRFTMKeys(string _name)
    {
        lock (noteReadLock)
        {
            string line;

            string rftmFileName = _name + ".rftm";
            string rftmFilePath = Application.streamingAssetsPath + @"/" + rftmFileName;

            int maxKey = 0;

            #region Read .rftm data
            if (File.Exists(rftmFilePath))
            {
                //Read each line, split with a array string
                //Name it separator
                //Then use string.Split(separator, ...)
                const char separator = ',';
                int filePosition = 0;
                int targetPosition = mainIO.InRFTMJumpTo("Objects", m_name);
                using (StreamReader rftmReader = new StreamReader(rftmFilePath))
                {
                    while (true)
                    {
                        line = rftmReader.ReadLine();
                        if (line == null)
                            return;

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

                            newNoteObj.instID = Convert.ToInt32(line.Split(separator)[0]);
                            newNoteObj.instSample = Convert.ToInt32(line.Split(separator)[1]);
                            newNoteObj.instType = (NoteObj.NoteObjType)Convert.ToInt32(line.Split(separator)[2]);

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
                            if (newNoteObj.instID > maxKey)
                            {
                                maxKey = newNoteObj.instID;
                                totalKeys = maxKey + 1;
                            }

                            //Lastly, add our new key to the list
                            noteObjs.Add(newNoteObj);

                            //We'll be integrating our new Object Readers around this area.
                            //We'll distribute the basic values to each one.
                            #region Distribution to Readers
                            switch (newNoteObj.instType)
                            {
                                case NoteObj.NoteObjType.Tap:
                                    DistributeTypeTo(tapObjectReader, newNoteObj);
                                    break;

                                case NoteObj.NoteObjType.Hold:
                                    DistributeTypeTo(holdObjectReader, newNoteObj);
                                    break;

                                case NoteObj.NoteObjType.Burst:
                                    DistributeTypeTo(slideObjectReader, newNoteObj);
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

    float CalculateDifficultyRating()
    {
        int totalNotes = noteObjs.Count;
        float songLengthInSec = RoftPlayer.musicSource.clip.length;
        float notesPerSec = (totalNotes / songLengthInSec);
        float totalKeys = keyLayoutClass.primaryBindedKeys.Count;
        float approachSpeedInPercent = (float)NoteEffector.Instance.approachSpeed / 100;
        float gameModeBoost = 0;
        const int maxKeys = 30;

        float calculatedRating = notesPerSec +
            (totalKeys / maxKeys) +
            approachSpeedInPercent +
            (RoftPlayer.musicSource.pitch / 2) +
            gameModeBoost;

        return calculatedRating;
    }

    long CalculateMaxScore()
    {
        long ini_Score = 0;
        for (long ini_Combo = 1; ini_Combo < totalNotes + 1; ini_Combo++)
        {
            ini_Score += 300 * ini_Combo;
        }
        return ini_Score;
    }

    void KeyLayoutAwake()
    {

        int difficultyTag = mainIO.InRFTMJumpTo("Difficulty", m_name);
        int keyCount = mainIO.ReadPropertyFrom<int>(difficultyTag, "KeyCount", m_name);
        totalKeys = keyCount;
        if (!keyLayoutClass.gameObject.activeInHierarchy)
        {
            keyLayoutClass.gameObject.SetActive(true);
        }

        #region Reading KeyCount

        switch (keyCount)
        {
            case 4:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_1x4;
                break;
            case 8:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_2x4;
                break;
            case 12:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_3x4;
                break;
            case 16:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_4x4;
                break;
        }
        #endregion

        if
           (GameManager.Instance.gameMode == GameManager.GameMode.TECHMEISTER ||
            GameManager.Instance.gameMode == GameManager.GameMode.STANDARD)
            Key_Layout.Instance.SetUpLayout();

    }


    void DistributeTypeTo(ObjectTypes _objectReader, NoteObj _key)
    {
        if (_objectReader != null)
            _objectReader.objects.Add(_key);
    }
}
