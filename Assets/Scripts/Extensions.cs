using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Extensions
{
    public static class String
    {
        #region Extensions
        public static string TryConcat(this string _, params string[] strings)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                foreach (string _string in strings)
                {
                    stringBuilder = stringBuilder.AppendLine(_string);
                }
                return stringBuilder.ToString();
            }
            catch (IOException e)
            {
                Debug.Log(string.Format("Failed to concatenate: {0}", e.Message));
                return string.Empty;
            }
        }
    }

    public static class File
    {
        public static void TryCopy(string sourceName, string destination)
        {
            try
            {
                System.IO.File.Copy(sourceName, destination);
            }
            catch (IOException e)
            {
                Debug.Log(string.Format("Failed to Copy File Info: {0}", e.Message));
                return;
            }
        }
        #endregion
    }
}

