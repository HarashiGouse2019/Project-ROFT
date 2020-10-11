using ROFTIOMANAGEMENT;

public class RoftFormat
{
    const string newLine = "\n";

    #region Format Version
    static string t_version = "Format Version".AsTag();
    static string p_formatVer = "1.4v" + newLine;
    #endregion

    #region [General]
    static string t_general = "General".AsTag();
    static string p_Author = "Author".AsProperty(System.Environment.UserName) + newLine;
    static string p_AudioFileName = "AudioFilename".AsProperty(RoftCreator.audioFilePath) + newLine;
    static string p_BackgroundImage = "BackgroundImage".AsProperty(RoftCreator.backgroundFilePath) + newLine;
    static string p_BackgroundVideo = "BackgroundVideo".AsProperty() + newLine;
    #endregion

    #region [Metadata]
    static string t_metadata = "Metadata".AsTag();
    static string p_Title = "Title".AsProperty(RoftCreator.GetSongTitle()) + newLine;
    static string p_TitleUnicode = "TitleUnicode".AsProperty(RoftCreator.GetSongTitle(true)) + newLine;
    static string p_Artist = "Artist".AsProperty(RoftCreator.GetSongArtist()) + newLine;
    static string p_ArtistUnicode = "ArtistUnicode".AsProperty(RoftCreator.GetSongArtist(true)) + newLine;
    static string p_Creator = "Creator".AsProperty(System.Environment.UserName) + newLine;
    static string p_ROFTID = "ROFTID".AsProperty(RoftCreator.GetROFTID()) + newLine;
    static string p_GROUPID = "GROUPID".AsProperty(RoftCreator.GetGROUPID()) + newLine;
    #endregion

    #region [Difficulty]
    static string t_difficulty = "Difficulty".AsTag();
    static string p_DifficultyName = "DifficultyName".AsProperty(RoftCreator.GetDifficultyName()) + newLine;
    static string p_StressBuild = "StressBuild".AsProperty(RoftCreator.GetStressBuild().ToString()) + newLine;
    static string p_ObjectCount = "ObjectCount".AsProperty(ObjectLogger.Instance != null ? ObjectLogger.GetObjectCount() : 0) + newLine;
    #region Key Count
    static string keyInfo = GetLayoutType();


    #endregion
    static string p_KeyCount = "KeyLayout".AsProperty(keyInfo) + newLine;

    static string p_AccuracyHarshness = "AccuracyHarshness".AsProperty(RoftCreator.GetAccuracyHarshness()) + newLine;
    static string p_ApproachSpeed = "ApproachSpeed".AsProperty(RoftCreator.GetApproachSpeed()) + newLine;
    #endregion

    #region [Timing]
    static string t_timing = "Timing".AsTag() + newLine;
    #endregion

    #region [Objects]
    static string t_objects = "Objects".AsTag() + newLine;
    static string objectData = ObjectLogger.Instance != null ? ObjectLogger.ObjectData : "NIL";
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

                   //Timing
                   t_timing,

                   //Difficulty
                   t_difficulty +
                   p_DifficultyName +
                   p_StressBuild +
                   p_ObjectCount +
                   p_KeyCount +
                   p_AccuracyHarshness +
                   p_ApproachSpeed,

                   //Objects
                   t_objects +
                   objectData
    };
    #endregion

    /// <summary>
    /// Get Layouttype used in generating format.
    /// </summary>
    /// <returns>Layout type, or how many keys are used.</returns>
    static string GetLayoutType() => RoftCreator.GetTotalKeys().ToString();

    /// <summary>
    /// Get the newly generated format as an array.
    /// </summary>
    /// <returns>An array of information that include tags and their properties.</returns>
    public string[] GetFormatInfo() => rftmInformation;


}
