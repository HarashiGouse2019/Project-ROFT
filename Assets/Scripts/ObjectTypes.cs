using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class ObjectTypes : MonoBehaviour, IReadType
{
    public List<NoteObj> objects { get; set; } = new List<NoteObj>();

    [SerializeField]
    public long sequencePos { get; set; } = 0;

    public NoteObj.NoteObjType readerType { get; set; }

    private const uint reset = 0;

    //This will be responsible for reading different types of the same file
    public virtual void ReadTapsFromFile()
    {

    }

    public virtual void ReadHoldsFromFile()
    {

    }

    public virtual void ReadSlidesFromFile()
    {

    }

    public virtual void ReadClicksFromFile()
    {

    }

    public virtual void ReadTrailsFromFile()
    {

    }

    public virtual long GetSequencePosition() => sequencePos;


    public virtual void SequencePositionReset()
    {
        sequencePos = reset;
    }

    public virtual GameObject GetTypeFromPool(ObjectPooler _pooler)
    {
        switch (readerType)
        {
            case NoteObj.NoteObjType.Tap:
                return _pooler.GetMember("Approach Circle");
            case NoteObj.NoteObjType.Hold:
                return _pooler.GetMember("Approach Circle");
            case NoteObj.NoteObjType.Burst:
                return _pooler.GetMember("Approach Circle");
            default:
                return null;
        }
    }

    public virtual void Next()
    {
        sequencePos++;
    }

    public virtual void SetToType(NoteObj.NoteObjType type)
    {
        readerType = type;
    }
}
