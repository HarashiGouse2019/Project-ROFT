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

    public int keyLayoutEnum;

    public List<Key> keys = new List<Key>();

    public Key_Layout keyLayoutClass = Key_Layout.Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ReadRFTMKeys(m_name);
        int thisPosition = InRFTMJumpTo("General");
        Debug.Log((string)ReadPropertyFrom(thisPosition, "AudioFilename"));
        KeyLayoutAwake();
    }

    private void Update()
    {
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
                        //We create a new key, and assign our data value to our key
                        Key newKey = new Key
                        {
                            keyNum = Convert.ToInt32(line.Split(separator)[0]),
                            keySample = Convert.ToInt32(line.Split(separator)[1]),
                            type = (Key.KeyType)Convert.ToInt32(line.Split(separator)[2])
                        };

                        //Update maxKeys
                        if (newKey.keyNum > maxKey)
                        {
                            maxKey = newKey.keyNum;
                            totalKeys = maxKey + 1;
                        }

                        //Lastly, add our new key to the list
                        keys.Add(newKey);

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
        int targetTag = InRFTMJumpTo("Difficulty");

        int totalNotes = keys.Count;
        float songLengthInSec = EditorToolClass.musicSource.clip.length;
        float notesPerSec = (totalNotes / songLengthInSec);
        float totalKeys = keyLayoutClass.bindedKeys.Count;
        float approachSpeedInPercent = (float)NoteEffect.Instance.approachSpeed / 100;
        const int maxKeys = 30;

        float calculatedRating = notesPerSec + (totalKeys / maxKeys) + approachSpeedInPercent + (EditorToolClass.musicSource.pitch / 2);

        return calculatedRating;
    }

    void KeyLayoutAwake()
    {
        if (!keyLayoutClass.gameObject.activeInHierarchy)
            keyLayoutClass.gameObject.SetActive(true);
        switch (totalKeys)
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
            case 30:
                keyLayoutClass.keyLayout = Key_Layout.KeyLayoutType.Layout_3Row;
                break;
        }
        keyLayoutClass.SetUpLayout();
    }

    int InRFTMJumpTo(string _tag, string _fileName = "", string _fileDirectory = "")
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

    string ReadPropertyFrom(int _startPosition, string _property)
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
}
