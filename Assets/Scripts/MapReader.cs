using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    public static MapReader Instance;

    public string m_name;

    public List<Key> keys = new List<Key>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ReadRFTM(m_name);
    }

    void ReadRFTM(string _name)
    {
        string line;

        string rftmFileName = _name + ".rftm";
        string rftmFilePath = Application.streamingAssetsPath + @"/" + rftmFileName;


        #region Read .rftm data
        if (File.Exists(rftmFilePath))
        {
            Debug.Log("Reading..." + _name);
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
                    Key newKey = new Key();
                    newKey.keyNum = Convert.ToInt32(line.Split(separator)[0]);
                    newKey.keySample = Convert.ToInt32(line.Split(separator)[1]);
                    newKey.type = (Key.KeyType)Convert.ToInt32(line.Split(separator)[2]);

                    //Lastly, add our new key to the list
                    keys.Add(newKey);
                }
            }
        }
        #endregion

        
    }
}
