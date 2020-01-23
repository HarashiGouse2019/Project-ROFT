﻿using System;
using System.IO;
using UnityEngine;

[SerializeField]
public class RoftIO
{
    public int InRFTMJumpTo(string _tag, string m_name, string _fileName = "", string _fileDirectory = "")
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
                        Debug.LogWarning("The tag " + _tag + " doesn't exist.");
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

        Debug.LogWarning("The specified file doesn't exist: " + _fileDirectory);
        #endregion

        return -1;
    }

    public T ReadPropertyFrom<T>(int _startPosition, string _property, string m_name)
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
                        return (T)Convert.ChangeType(line, typeof(T));
                    }

                    if (position > _startPosition)
                    {
                        if (line == "")
                            return (T)Convert.DBNull;

                        //If not empty, you can do whatever!!!
                        if (line.Contains(_property))
                        {
                            foundProperty = true;
                            targetVal = line.Replace(_property + ": ", "");

                            return (T)Convert.ChangeType(targetVal, typeof(T));
                        }
                    }
                    position++;
                }
            }
        }
        return (T)Convert.DBNull;
    }

}