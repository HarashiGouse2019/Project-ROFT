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
    public int instID;
    public int instSample;
    public NoteObjType instType;

    //Miscellaneous Values
    [Header("Miscellaneous Values")]
    public int miscellaneousValue1;
    public int miscellaneousValue2;
}

