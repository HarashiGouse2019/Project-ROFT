[System.Serializable]
public class BurstObjectReader : ObjectTypes
{
    public BurstObjectReader()
    {
        SetToType(NoteObj.NoteObjType.Burst);
    }

    public override void ReadSlidesFromFile()
    {
        
        base.ReadSlidesFromFile();
    }
}
