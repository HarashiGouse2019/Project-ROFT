using UnityEngine;
using System.Collections.Generic;
public interface IReadType
{
    List<NoteObj> objects { get; set; }

    [SerializeField]
    long sequencePos { get; set;}

    NoteObj.NoteObjType readerType { get; set; }

    //This will be responsible for reading different types of the same file
    void ReadTapsFromFile();

    void ReadHoldsFromFile();
    void ReadSlidesFromFile();
    void ReadClicksFromFile();
    void ReadTrailsFromFile();

    long GetSequencePosition();


    void SequencePositionReset();

    GameObject GetTypeFromPool(ObjectPooler _pooler);

    void Next();

    void SetToType(NoteObj.NoteObjType type);
}
