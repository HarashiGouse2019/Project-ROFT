using System;
using UnityEngine;

[Serializable]
public class NoteObj
{
    public enum NoteObjType
    {
        Tap,
        Hold,
        Track,
        Burst
    }

    //Central Values
    [Header("Central Values")]

    //NoteInstanceID
    [SerializeField] private uint instID;
    [SerializeField] private long instSample;
    [SerializeField] private NoteObjType instType;

    //Miscellaneous Values
    [Header("Miscellaneous Values")]
    public object miscellaneousValue1;
    public object miscellaneousValue2;

    public void SetKeyID(uint value)
    {
        instID = value;
    }

    public void SetInitialSample(long value)
    {
        instSample = value;
    }

    public void SetType(NoteObjType type)
    {
        instType = type;
    }

    public uint GetKey() => instID;

    public long GetInitialeSample() => instSample;

    public NoteObjType GetType() => instType;

    public string AsString()
    {
        switch (instType)
        {
            case NoteObjType.Tap: return string.Format("{0},{1},{2}", instID, instSample, (int)instType);
            case NoteObjType.Hold: return string.Format("{0},{1},{2},{3}", instID, instSample, (int)instType, null);
            case NoteObjType.Track: return string.Format("{0},{1},{2},{3}", instID, instSample, (int)instType, null);
            case NoteObjType.Burst: return string.Format("{0},{1},{2},{3}", instID, instSample, (int)instType, null);
        }
        return string.Empty;
    }

    public  bool Empty()
    {
        return (instID == 0 && instSample == 0 && instType == default);
    }

    public void Clear()
    {
        instID = 0;
        instSample = 0;
        instType = default;
        miscellaneousValue1 = null;
        miscellaneousValue2 = null;
    }
}

