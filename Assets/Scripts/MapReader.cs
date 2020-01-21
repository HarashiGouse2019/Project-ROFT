using System;
using System.IO;
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

    public List<Key> keys = new List<Key>();

    public Key_Layout keyLayoutClass = Key_Layout.Instance;

    //All Object Readers
    [Header("Object Readers")]
    public TapObjectReader tapObjectReader;
    public HoldObjectReader holdObjectReader;
    public SlideObjectReader slideObjectReader;
    public TrailObjectReader trailObjectReader;
    public ClickObjectReader clickObjectReader;

    private void Awake()
    {
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
            KeyLayoutAwake((int)keyLayoutClass.layoutMethod);

        //Get other values such as Approach Speed, Stress Build, and Accuracy Harshness
        if (RoftPlayer.Instance != null && !RoftPlayer.Instance.record)
        {
            int difficultyTag = InRFTMJumpTo("Difficulty");
            NoteEffector.Instance.approachSpeed = float.Parse(ReadPropertyFrom(difficultyTag, "ApproachSpeed"));
            GameManager.Instance.stressBuild = float.Parse(ReadPropertyFrom(difficultyTag, "StressBuild"));
            NoteEffector.Instance.accuracy = float.Parse(ReadPropertyFrom(difficultyTag, "AccuracyHarshness"));
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
            int targetPosition = InRFTMJumpTo("Objects");
            using (StreamReader rftmReader = new StreamReader(rftmFilePath))
            {
                while (true)
                {
                    line = rftmReader.ReadLine();
                    if (line == null)
                        return;

                    if (filePosition > targetPosition)
                    {
                        //We'll count the frequency of commas to determine
                        //that they are more values in the object
                        int countCommas = 0;
                        foreach (char c in line)
                            if (c == ',')
                                countCommas++;

                        //We create a new key, and assign our data value to our key
                        Key newKey = new Key();

                        newKey.keyNum = Convert.ToInt32(line.Split(separator)[0]);
                        newKey.keySample = Convert.ToInt32(line.Split(separator)[1]);
                        newKey.type = (Key.KeyType)Convert.ToInt32(line.Split(separator)[2]);

                        //Check for any miscellaneous values
                        if (countCommas > 2)
                            newKey.miscellaneousValue1 = Convert.ToInt32(line.Split(separator)[3]);

                        else if (countCommas > 3)
                            newKey.miscellaneousValue2 = Convert.ToInt32(line.Split(separator)[4]);

                        //Update maxKeys
                        if (newKey.keyNum > maxKey)
                        {
                            maxKey = newKey.keyNum;
                            totalKeys = maxKey + 1;
                        }

                        //Lastly, add our new key to the list
                        keys.Add(newKey);

                        //We'll be integrating our new Object Readers around this area.
                        //We'll distribute the basic values to each one.
                        #region Distribution to Readers
                        switch (newKey.type)
                        {
                            case Key.KeyType.Tap:
                                DistributeTypeTo(tapObjectReader, newKey);
                                break;

                            case Key.KeyType.Hold:
                                DistributeTypeTo(holdObjectReader, newKey);
                                break;

                            case Key.KeyType.Slide:
                                DistributeTypeTo(slideObjectReader, newKey);
                                break;

                            case Key.KeyType.Trail:
                                DistributeTypeTo(trailObjectReader, newKey);
                                break;

                            case Key.KeyType.Click:
                                DistributeTypeTo(clickObjectReader, newKey);
                                break;

                            default:
                                break;
                        }
                        #endregion

                        //Update total Notes
                        totalNotes = keys.Count;
                    }
                    filePosition++;
                }
            }
        }
        #endregion


    }

    float CalculateDifficultyRating()
    {
        int totalNotes = keys.Count;
        float songLengthInSec = RoftPlayer.musicSource.clip.length;
        float notesPerSec = (totalNotes / songLengthInSec);
        float totalKeys = keyLayoutClass.primaryBindedKeys.Count;
        float approachSpeedInPercent = (float)NoteEffector.Instance.approachSpeed / 100;
        float gameModeBoost = 0;
        const int maxKeys = 30;

        if (keyLayoutClass.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter)
            gameModeBoost = 2.0f;

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

    void KeyLayoutAwake(int _appearEffectOn)
    {
        int difficultyTag = InRFTMJumpTo("Difficulty");
        int keyCount = Convert.ToInt32(ReadPropertyFrom(difficultyTag, "KeyCount"));
        totalKeys = keyCount;
        if (!keyLayoutClass.gameObject.activeInHierarchy)
        {
            keyLayoutClass.gameObject.SetActive(true);
            if (_appearEffectOn == 1)
                keyLayoutClass.GetComponent<AppearEffect>().enabled = keyLayoutClass.gameObject.activeInHierarchy;
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
            case 10:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_HomeRow;
                break;
            case 12:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_3x4;
                break;
            case 16:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_4x4;
                break;
            case 30:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_3Row;
                break;
        } 
        #endregion

        if
           (GameManager.Instance.gameMode == GameManager.GameMode.TECHMEISTER ||
            GameManager.Instance.gameMode == GameManager.GameMode.STANDARD)
            Key_Layout.Instance.SetUpLayout();
    }

    public int InRFTMJumpTo(string _tag, string _fileName = "", string _fileDirectory = "")
    {
        //Setting default for directory
        if (_fileDirectory == "")
            _fileDirectory = Application.streamingAssetsPath;

        //Setting default for file name
        if (_fileName == "" && m_name != null)
            _fileName = m_name;

        string line;

        string rftmFileName = _fileName + ".rftm";
        string rftmFilePath = _fileDirectory + @"/" + rftmFileName;


        int targetPosition = 0;
        #region Read .rftm data
        if (File.Exists(rftmFilePath))
        {
            //Read each line, split with a array string
            //Name it separator
            //Then use string.Split(separator, ...)
            char[] separator = { '[', ']' };
            using (StreamReader rftmReader = new StreamReader(rftmFilePath))
            {
                while (true)
                {
                    line = rftmReader.ReadLine();

                    if (line == null)
                    {
                        Debug.Log("The tag " + _tag + " doesn't exist.");
                        return -1;
                    }

                    line.Split(separator);
                    if (line.Contains(_tag))
                    {
                        return targetPosition;
                    }
                    targetPosition++;
                }
            }
        }

        Debug.Log("The specified file doesn't exist: " + _fileDirectory);
        #endregion

        return -1;
    }

    public string ReadPropertyFrom(int _startPosition, string _property)
    {
        string line;
        string rftmFilePath = Application.streamingAssetsPath + @"/" + m_name + ".rftm";
        int position = 0;

        //Check if it actually found something
        bool foundProperty = false;

        if (File.Exists(rftmFilePath))
        {
            using (StreamReader rftmReader = new StreamReader(rftmFilePath))
            {
                while (true)
                {
                    string targetVal = null;

                    line = rftmReader.ReadLine();

                    if (line == null)
                    {

                        if (!foundProperty) Debug.Log("This tag does not exist");
                        return line;
                    }

                    if (position > _startPosition)
                    {
                        if (line == "")
                            return null;

                        //If not empty, you can do whatever!!!
                        if (line.Contains(_property))
                        {
                            foundProperty = true;
                            targetVal = line.Replace(_property + ": ", "");

                            return targetVal;
                        }
                    }
                    position++;
                }
            }
        }
        return null;
    }

    void DistributeTypeTo(ObjectTypes _objectReader, Key _key)
    {
        if (_objectReader != null)
            _objectReader.objects.Add(_key);
    }
}
