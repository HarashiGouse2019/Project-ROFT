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
        ReadRFTM(m_name);
        KeyLayoutAwake();
    }

    private void Update()
    {
        difficultyRating = CalculateDifficultyRating();
    }

    void ReadRFTM(string _name)
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
            using (StreamReader rftmReader = new StreamReader(rftmFilePath))
            {
                while (true)
                {
                    line = rftmReader.ReadLine();
                    if (line == null)
                        return;
                    //We create a new key, and assign our data value to our key
                    Key newKey = new Key
                    {
                        keyNum = Convert.ToInt32(line.Split(separator)[0]),
                        keySample = Convert.ToInt32(line.Split(separator)[1]),
                        type = (Key.KeyType)Convert.ToInt32(line.Split(separator)[2])
                    };

                    //Update maxKeys
                    if (newKey.keyNum > maxKey) {
                        maxKey = newKey.keyNum;
                        totalKeys = maxKey + 1;
                    }

                    //Lastly, add our new key to the list
                    keys.Add(newKey);

                    //Update total Notes
                    totalNotes = keys.Count;
                }
            }   
        }
        #endregion
    }

    float CalculateDifficultyRating()
    {
        int totalNotes = keys.Count;
        float songLengthInSec = EditorToolClass.musicSource.clip.length;
        float notesPerSec = (totalNotes / songLengthInSec);
        float totalKeys = Key_Layout.keyObjects.Count;
        float approachSpeedInPercent = (float)NoteEffect.Instance.approachSpeed / 100;
        const int maxKeys = 30;

        float calculatedRating = notesPerSec + (totalKeys / maxKeys) + approachSpeedInPercent;

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
}
