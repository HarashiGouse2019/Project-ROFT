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
}

