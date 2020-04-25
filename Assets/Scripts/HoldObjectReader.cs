[System.Serializable]
public class HoldObjectReader : ObjectTypes
{

    public HoldObjectReader()
    {
        SetToType(NoteObj.NoteObjType.Hold);
    }

    public override void ReadHoldsFromFile()
    {
        base.ReadHoldsFromFile();
    }
}
