using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    public static MapReader Instance;

    public string m_name;

    public float difficultyRating;

    public List<Key> keys = new List<Key>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ReadRFTM(m_name);
        difficultyRating = CalculateDifficultyRating();
    }

    void ReadRFTM(string _name)
    {
        string line;

        string rftmFileName = _name + ".rftm";
        string rftmFilePath = Application.streamingAssetsPath + @"/" + rftmFileName;


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

                    //Lastly, add our new key to the list
                    keys.Add(newKey);
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
        const int maxKeys = 30;

        Debug.Log(songLengthInSec);

        float calculatedRating = notesPerSec + (totalKeys / maxKeys);

        return calculatedRating;
    }
}
