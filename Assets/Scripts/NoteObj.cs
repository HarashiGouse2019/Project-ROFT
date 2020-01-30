using System;
using UnityEngine;

[Serializable]
public class NoteObj
{ 
    public enum NoteObjType
    {
        Tap,
        Hold,
        Burst
    }

    //Central Values
    [Header("Central Values")]

    //NoteInstanceID
    private uint instID;
    private long instSample;
    private NoteObjType instType;

    //Miscellaneous Values
    [Header("Miscellaneous Values")]
    public object miscellaneousValue1;
    public object miscellaneousValue2;

    public void SetInstanceID(uint value)
    {
        instID = value;
    }

    public void SetInstanceSample(long value)
    {
        instSample = value;
    }

    public void SetInstanceType(NoteObjType type)
    {
        instType = type;
    }

    public uint GetInstanceID()
    {
        return instID;
    }

    public long GetInstanceSample()
    {
        return instSample;
    }

    public NoteObjType GetInstanceType()
    {
        return instType;
    }
}

