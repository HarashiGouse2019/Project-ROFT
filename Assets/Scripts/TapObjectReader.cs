using System.Collections;
using System.Collections.Generic;
[System.Serializable]

public class TapObjectReader : ObjectTypes
{
    public TapObjectReader()
    {
        SetToType(NoteObj.NoteObjType.Tap);
    }

    public override void ReadTapsFromFile()
    {
        base.ReadTapsFromFile();
    }
}
