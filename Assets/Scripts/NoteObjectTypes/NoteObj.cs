using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoteObj
{
    public sealed class TrackPoint
    {
        public uint connectionKey;
        public long connectionSample;
        public TrackPoint(uint connectionKeyValue, long connectionSampleValue)
        {
            connectionKey = connectionKeyValue;
            connectionSample = connectionSampleValue;
        }
    }

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

    //Miscellaneous Values
    [Header("Miscellaneous Values")]
    public object miscellaneousValue1;
    public object miscellaneousValue2;

    //Miscellaneous data for Hold Type
    protected long finalSample = -1;

    //Misscellaneous data for Track Type
    protected List<TrackPoint> points;

    //Misscellaneous data for Burst Type
    protected uint direction;

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
        miscellaneousValue1 = null;
        miscellaneousValue2 = null;
    }
}

