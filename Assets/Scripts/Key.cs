using System;
using UnityEngine;

[Serializable]
public class Key
{ 
    public enum KeyType
    {
        Tap,
        Hold,
        Slide,
        Trail,
        Click
    }

    //Central Values
    [Header("Central Values")]
    public int keyNum;
    public int keySample;
    public KeyType type;

    //Miscellaneous Values
    [Header("Miscellaneous Values")]
    public int miscellaneousValue1;
    public int miscellaneousValue2;
}

