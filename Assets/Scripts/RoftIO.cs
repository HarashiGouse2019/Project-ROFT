using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;

using Random = UnityEngine.Random;


namespace ROFTIOMANAGEMENT
{
    public static class ROFTFormat
    {
        const string newLine = "\n";

        #region Format Version
        static string t_version = "[Format Version]\n";
        static string p_formatVer = "1.0v" + newLine;
        #endregion

        #region [General]
        static string t_general = "[General]\n";
        static string p_Author = "Author: " + System.Environment.UserName + newLine;
        static string p_AudioFileName = "AudioFilename: " + RoftCreator.audioFilePath + newLine;
        static string p_BackgroundImage = "BackgroundImage: " + RoftCreator.backgroundFilePath + newLine;
        static string p_BackgroundVideo = "BackgroundVideo: " + newLine;
        #endregion

        #region [Metadata]
        static string t_metadata = "[Metadata]\n";
        static string p_Title = "Title: " + RoftCreator.Instance.GetSongTitle() + newLine;
        static string p_TitleUnicode = "TitleUnicode: " + RoftCreator.Instance.GetSongTitle(true) + newLine;
        static string p_Artist = "Artist: " + RoftCreator.Instance.GetSongArtist() + newLine;
        static string p_ArtistUnicode = "ArtistUnicode: " + RoftCreator.Instance.GetSongArtist(true) + newLine;
        static string p_Creator = "Creator: " + System.Environment.UserName + newLine;
        static string p_ROFTID = "ROFTID: " + RoftCreator.Instance.GetROFTID() + newLine;
        static string p_GROUPID = "GROUPID: " + RoftCreator.Instance.GetGROUPID() + newLine;
        #endregion

        #region [Difficulty]
        static string t_difficulty = "[Difficulty]\n";
        static string p_DifficultyName = "DifficultyName: " + RoftCreator.Instance.GetDifficultyName() + newLine;
        static string p_StressBuild = "StressBuild: " + GameManager.Instance.stressBuild.ToString() + newLine;
        static string p_ObjectCount = "ObjectCount: " + newLine;
        #region Key Count
        static string keyInfo = GetLayoutType();


        #endregion
        static string p_KeyCount = "KeyCount: " + keyInfo + newLine;

        static string p_AccuracyHarshness = "AccuracyHarshness: " + RoftCreator.Instance.GetAccuracyHarshness() + newLine;
        static string p_ApproachSpeed = "ApproachSpeed: " + RoftCreator.Instance.GetApproachSpeed() + newLine;
        #endregion

        #region [Objects]
        static string t_objects = "[Objects]";
        #endregion

        #region .rftm Information
        static string[] rftmInformation = new string[]
        {
                   //Format Version
                   t_version +
                   p_formatVer,

                   //General
                   t_general +
                   p_Author +
                   p_AudioFileName +
                   p_BackgroundImage +
                   p_BackgroundVideo,

                   //Metadata
                   t_metadata +
                   p_Title +
                   p_TitleUnicode +
                   p_Artist +
                   p_ArtistUnicode +
                   p_Creator +
                   p_ROFTID +
                   p_GROUPID,

                   //Difficulty
                   t_difficulty +
                   p_DifficultyName +
                   p_StressBuild +
                   p_ObjectCount +
                   p_KeyCount +
                   p_AccuracyHarshness +
                   p_ApproachSpeed,

                   //Objects
                   t_objects
        };
        #endregion

        static string GetLayoutType()
        {
            switch (RoftCreator.Instance.GetTotalKeys())
            {
                case Key_Layout.KeyLayoutType.Layout_1x4:
                    return "4";
                case Key_Layout.KeyLayoutType.Layout_2x4:
                    return "8";
                case Key_Layout.KeyLayoutType.Layout_3x4:
                    return "12";
                case Key_Layout.KeyLayoutType.Layout_4x4:
                    return "16";
                default:
                    return "";
            }
        }

        public static string[] GetFormatInfo() => rftmInformation;
    }

    [SerializeField]
    public static class IDHIST
    {
        private static string iHistPath { get; } = Application.persistentDataPath + "/ihist.ID";

        /// <summary>
        /// Create a new Identity histroy
        /// </summary>
        public static void NewHistory()
        {
            File.CreateText(iHistPath);
        }

        /// <summary>
        /// Check if Identity History exists.
        /// </summary>
        /// <returns></returns>
        public static bool HistoryExists()
        {
            bool exist = File.Exists(iHistPath);
            return exist;
        }

        /// <summary>
        /// Write a new ID into history file.
        /// </summary>
        /// <param name="_content"></param>
        public static void Write(string _content)
        {
            //We'll reference to the file ihist file.
            //This file will keep track of all the used IDs.
            string iHistPath = Application.persistentDataPath + "/ihist.ID";
            byte[] bytesToEncode = Encoding.UTF32.GetBytes(_content);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            if (File.Exists(iHistPath))
            {
                StreamWriter streamWriter = File.AppendText(iHistPath);
                streamWriter.Write(encodedText + "\n");
                streamWriter.Close();
            }
            else
            {
                Debug.Log("ID History File doesn't exist.");
                Application.Quit();
            }
        }

        /// <summary>
        /// Iterate through ihist.ID and return an array of data
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllID()
        {
            List<string> data = new List<string>();
            for(int id = 0; id < File.ReadAllLines(iHistPath).Length; id++)
            {
                byte[] decodedBytes = Convert.FromBase64String(File.ReadAllLines(iHistPath)[id]);
                string encodedText = Encoding.UTF32.GetString(decodedBytes);
                data.Add(encodedText);
            }
            return data.ToArray();
        }

        /// <summary>
        /// Check if the specified GroupID is used in our ID history
        /// </summary>
        /// <param name="_groupID">The GroupId for the method to search for.</param>
        /// <returns>If the GroupID is used.</returns>
        public static bool GROUPIDExists(long _groupID)
        {
            //We'll have to iterate through out ihist.ID file, and parse for GROUPID
            string[] data = GetAllID();

            char[] delimiters = { '-', '(', ')' };
            foreach (string id in data)
            {
                if (_groupID == Convert.ToInt64(id.Split(delimiters)[1]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_roftID">The specified RoftID to search for.</param>
        /// <param name="_groupID">The GroupID that the RoftID is associated with.</param>
        /// <returns>If the RoftID is used.</returns>
        public static bool ROFTIDExistsInGroupID(int _roftID, long _groupID)
        {
            string[] data = GetAllID();

            char[] delimiters = { '-', '(', ')' };

            //Iterate and find matching GroupId first
            foreach(string id in data)
            {
                if (GROUPIDExists(_groupID) && id.Contains(_groupID + "-" + _roftID))
                    return true;
            }
            return false;
        }
    }

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

                    string[] rftmInfo = ROFTFormat.GetFormatInfo();

                    for (int line = 0; line < rftmInfo.Length; line++)
                        rftmWriter.WriteLine(rftmInfo[line]);
                }
                Debug.Log(".rftm file created!");
            }
        }

        public static int GenerateROFTID(long _groupID)
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

            //We will then iterate the ihist file and check if a roftID exists
            //This has to be in a specified GroupID.
            for (int uniqueNum = numRange[0]; uniqueNum < numRange[1] + 1; uniqueNum++)
            {
                if (!IDHIST.ROFTIDExistsInGroupID(uniqueNum, _groupID))
                {
                    stringROFTID += (uniqueNum.ToString());
                    return Convert.ToInt32(stringROFTID);
                }
            }

            return numRange[0];
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

        public static string GenerateDirectory(long? _GROUPID, string _songArtist, string _songName)
        {
            string _path = Application.persistentDataPath + "/Songs/" + "(" + _GROUPID + ") " + _songArtist + "-" + _songName;
            //Create a Directory Specific for a song
            Directory.CreateDirectory(_path);
            return _path;
        }

        public static string GenerateDirectory(string _directoryName)
        {
            string _path = Application.persistentDataPath + "/" + _directoryName;
            //Create a Directory Specific for a song
            Directory.CreateDirectory(_path);
            return _path;
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