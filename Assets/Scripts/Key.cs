using System;

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

    public int keyNum;
    public int keySample;
    public KeyType type;
}

