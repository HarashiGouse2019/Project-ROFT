using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class ObjectTypes : MonoBehaviour
{
    public List<NoteObj> objects = new List<NoteObj>();

    [SerializeField]
    private long sequencePos = 0;

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

    public virtual long GetSequencePosition()
    {
        return sequencePos;
    }

    public virtual void Next()
    {
        sequencePos++;
    }
}
