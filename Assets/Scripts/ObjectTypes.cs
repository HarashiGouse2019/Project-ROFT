using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ObjectTypes : MonoBehaviour
{
    public List<NoteObj> objects = new List<NoteObj>();

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
}
