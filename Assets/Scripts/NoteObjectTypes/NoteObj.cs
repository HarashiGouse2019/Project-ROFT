using System;

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

    //NoteInstanceID
    protected uint initialKey;
    protected long initialSample;
    protected NoteObjType type; //Use this to Cast Type

    public virtual void SetKeyID(uint value)
    {
        initialKey = value;
    }

    public virtual void SetInitialSample(long value)
    {
        initialSample = value;
    }

    public virtual void SetType(NoteObjType type)
    {
        this.type = type;
    }

    public virtual uint GetKey() => initialKey;

    public virtual long GetInitialeSample() => initialSample;

    public virtual NoteObjType GetNoteType() => type;

    public virtual string AsString()
    {
        return string.Empty;
    }

    public virtual bool Empty()
    {
        return (initialKey == 0 && initialSample == 0 && type == default);
    }

    public virtual void Clear()
    {
        initialKey = 0;
        initialSample = 0;
        type = default;
    }
}

