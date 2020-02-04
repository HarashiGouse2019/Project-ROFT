using System;
using System.IO;
using UnityEngine;
using UnityEditor;

using Random = UnityEngine.Random;


namespace ROFTIOMANAGEMENT
{
    [SerializeField]
    public static class RoftIO
    {
        public static void CreateNewRFTM(string _name, string _path)
        {
            string rftmFileName = _name + ".rftm";
            string rftmFilePath = _path + rftmFileName;

            if (!File.Exists(rftmFilePath))
            {
                Debug.Log("Creating new .rftm file...");
                using (StreamWriter rftmWriter = File.CreateText(rftmFilePath))
                {
                    const string newLine = "\n";

                    #region [General]
                    string t_general = "[General]\n";
                    string p_Author = "Author: " + System.Environment.UserName + newLine;
                    string p_AudioFileName = "AudioFilename: " + AssetDatabase.GetAssetPath(RoftCreator.Instance.GetAudioFile()) + newLine;
                    string p_BackgroundImage = "BackgroundImage: " + RoftCreator.Instance.GetBackgroundImage().name + newLine;
                    string p_BackgroundVideo = "BackgroundVideo: " + newLine;
                    #endregion

                    #region [Metadata]
                    string t_metadata = "[Metadata]\n";
                    string p_Title = "Title: " + RoftCreator.Instance.GetSongTitle() + newLine;
                    string p_TitleUnicode = "TitleUnicode: " + RoftCreator.Instance.GetSongTitle(true) + newLine;
                    string p_Artist = "Artist: " + RoftCreator.Instance.GetSongArtist() + newLine;
                    string p_ArtistUnicode = "ArtistUnicode: " + RoftCreator.Instance.GetSongArtist(true) + newLine;
                    string p_Creator = "Creator: " + System.Environment.UserName + newLine;
                    string p_ROFTID = "ROFTID: " + RoftCreator.Instance.GetROFTID() + newLine;
                    string p_GROUPID = "GROUPID: " + RoftCreator.Instance.GetGROUPID() + newLine;
                    #endregion

                    #region [Difficulty]
                    string t_difficulty = "[Difficulty]\n";
                    string p_DifficultyName = "DifficultyName: " + RoftCreator.Instance.GetDifficultyName() + newLine;
                    string p_StressBuild = "StressBuild: " + GameManager.Instance.stressBuild.ToString() + newLine;
                    string p_ObjectCount = "ObjectCount: " + newLine;
                    string keyInfo = "";
                    #region Key Count

                    switch (RoftCreator.Instance.GetTotalKeys())
                    {
                        case Key_Layout.KeyLayoutType.Layout_1x4:
                            keyInfo = "4";
                            break;
                        case Key_Layout.KeyLayoutType.Layout_2x4:
                            keyInfo = "8";
                            break;
                        case Key_Layout.KeyLayoutType.Layout_3x4:
                            keyInfo = "12";
                            break;
                        case Key_Layout.KeyLayoutType.Layout_4x4:
                            keyInfo = "16";
                            break;
                        default:
                            break;
                    }
                    #endregion
                    string p_KeyCount = "KeyCount: " + keyInfo + newLine;

                    string p_AccuracyHarshness = "AccuracyHarshness: " + RoftCreator.Instance.GetAccuracyHarshness() + newLine;
                    string p_ApproachSpeed = "ApproachSpeed: " + RoftCreator.Instance.GetApproachSpeed() + newLine;
                    #endregion

                    #region [Objects]
                    string t_objects = "[Objects]";
                    #endregion

                    #region .rftm Information
                    string[] rftmInformation = new string[]
                    {
                   t_general +
                   p_Author +
                   p_AudioFileName +
                   p_BackgroundImage +
                   p_BackgroundVideo,

                   t_metadata +
                   p_Title +
                   p_TitleUnicode +
                   p_Artist +
                   p_ArtistUnicode +
                   p_Creator +
                   p_ROFTID +
                   p_GROUPID,

                   t_difficulty +
                   p_DifficultyName +
                   p_StressBuild +
                   p_ObjectCount +
                   p_KeyCount +
                   p_AccuracyHarshness +
                   p_ApproachSpeed,

                   t_objects
                    };
                    #endregion

                    for (int line = 0; line < rftmInformation.Length; line++)
                        rftmWriter.WriteLine(rftmInformation[line]);
                }
                Debug.Log(".rftm file created!");
            }
        }

        public static int GenerateROFTID()
        {
            /*ROFTID is as structured:
             * GROUPID-(100 to 999)
             * Example: 1805121000-841
             */

            int[] numRange =
            {
            100,
            999
        };

            string stringROFTID = "";

            DateTime date = DateTime.Now;

            int uniqueNum = Random.Range(numRange[0], numRange[1]);

            stringROFTID += (uniqueNum.ToString());

            return Convert.ToInt32(stringROFTID);
        }

        public static int GenerateGROUPID()
        {
            /*GROUPID is as structured:
             * yy-dd-mm-(1000 to 9990)
             * Example: 1805121000
             * 
             * G - GROUPID
             * 18 - 2018
             * 05 - 5th
             * 12 - Dec
             * Number 1000
             */

            int[] numRange =
            {
            1000,
            9999
        };

            string stringGROUPID = "";

            DateTime date = DateTime.Now;

            string yearDigit = date.ToString("yy");
            string dayDigit = date.ToString("dd");
            string monthDigit = date.ToString("mm");
            string uniqueNum = Random.Range(numRange[0], numRange[1]).ToString();

            stringGROUPID += (yearDigit + monthDigit + dayDigit + uniqueNum);

            return Convert.ToInt32(stringGROUPID);
        }

        public static void WriteToRFTM(string _name, string _path, string _data)
        {
            string rftmFileName = _name + ".rftm";
            string rftmFilePath = _path + rftmFileName;

            StreamWriter rftmWriter = File.AppendText(rftmFilePath);

            MapReader.Instance.SetName(_name);

            rftmWriter.WriteLine(_data);
            rftmWriter.Close();
        }

        public static int InRFTMJumpTo(string _tag, string m_name, string _fileName = "", string _fileDirectory = "")
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

        public static T ReadPropertyFrom<T>(int _startPosition, string _property, string m_name)
        {
            string line;
            string rftmFilePath = Application.streamingAssetsPath + @"/" + m_name + ".rftm";

            //This is used to iterate and find the tag specified by the _startPosition parameter.
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

                        /*If we iterate through the file and can not find
                         the tag needed to access its property, we
                         return default
                         */
                        if (line == null)
                        {
                            if (!foundProperty) Debug.Log("This tag does not exist");
                            return default;
                        }

                        /*If we're at the position where our tag is defined in file
                         we then check for our property.
                         */
                        if (position > _startPosition)
                        {
                            if (line == "")
                                return default;

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
            return default;
        }
    }
}