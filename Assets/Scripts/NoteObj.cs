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
    [SerializeField] private uint instID;
    [SerializeField] private long instSample;
    [SerializeField] private NoteObjType instType;

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

    public uint GetInstanceID() => instID;

    public long GetInstanceSample() => instSample;

    public NoteObjType GetInstanceType() => instType;
}

