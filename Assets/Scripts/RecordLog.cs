using Extensions;

public class RecordLog
{
    string recordString;
    const char seperator = '|';
    public RecordLog(long score, uint rank, uint combo, uint perfectCount, uint greatCount, uint goodCount, uint okCount, uint missCount, float accuracyPercentage, int resetCount, bool success)
    {
        string _string = string.Format
            ("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
            score, rank, combo, perfectCount, greatCount, goodCount, okCount, missCount, accuracyPercentage, resetCount, success.AsNumericValue());
        recordString = _string;
    }
    public void AddNewRecordToRFTM(string fileDirectory)
    {
        ROFTIOMANAGEMENT.RoftIO.AddRecord(recordString, fileDirectory);
    }
    string SplitStringAt(int index, char seperator) => recordString.Split(seperator)[index];
    public long GetScore() => long.Parse(SplitStringAt(0, seperator));
    public uint GetRankValue() => uint.Parse(SplitStringAt(1, seperator));

    public uint GetComboCount() => uint.Parse(SplitStringAt(2, seperator));

    public uint GetPerfectCount() => uint.Parse(SplitStringAt(3, seperator));

    public uint GetGreatCount() => uint.Parse(SplitStringAt(4, seperator));

    public uint GetGoodCount() => uint.Parse(SplitStringAt(5, seperator));
    public uint GetOkCount() => uint.Parse(SplitStringAt(6, seperator));

    public uint GetMissCount() => uint.Parse(SplitStringAt(7, seperator));

    public float GetAccuracyPercentage() => uint.Parse(SplitStringAt(8, seperator));

    public int GetResetCount() => int.Parse(SplitStringAt(9, seperator));
    public bool IsSuccessfulAttempt() => bool.Parse(SplitStringAt(10, seperator));
}
