using System;

[Serializable]
public class Key
{
    public Key() { }

    public enum KeyType
    {
        Tap,
        Hold
    }

    public int keyNum;
    public int keySample;
    public KeyType type;
}

