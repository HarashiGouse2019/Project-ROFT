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
    protected uint InitialKey;
    protected long InitialSample;
    protected NoteObjType Type; //Use this to Cast Type

    //Editor information
    protected int PatternSetValue;  //Pattern Set that this note is on
    protected int TickValue;        //The tick that it's positioned
    protected uint LayerValue;      //On what layer on a selected tick

    public NoteObj()
    {
        PatternSetValue = (int)ObjectLogger.CurrentPatternSet;
        TickValue = (int)ObjectLogger.TickValue;
        LayerValue = (uint)ObjectLogger.CurrentStack;
    }

    public virtual void SetInitialKey(uint value)
    {
        InitialKey = value;
    }

    public virtual void SetInitialSample(long value)
    {
        InitialSample = value;
    }

    public virtual void SetType(NoteObjType type)
    {
        Type = type;
    }

    public virtual void SetPatternSetValue(int value)
    {
        PatternSetValue = value;
    }

    public virtual void SetTickValue(int value)
    {
        TickValue = value;
    }

    public virtual void SetLayerValue(uint value)
    {
        LayerValue = value;
    }

    public virtual uint GetKey() => InitialKey;

    public virtual long GetInitialeSample() => InitialSample;

    public virtual NoteObjType GetNoteType() => Type;

    public virtual string AsString()
    {
        return string.Empty;
    }

    public virtual bool Empty()
    {
        return (InitialKey == 0 &&
                InitialSample == 0 &&
                Type == default &&
                PatternSetValue == 0 &&
                TickValue == 0 &&
                LayerValue == 0);
    }

    public virtual void Clear()
    {
        InitialKey = 0;
        InitialSample = 0;
        Type = default;
        PatternSetValue = 0;
        TickValue = 0;
        LayerValue = 0;
    }
}

