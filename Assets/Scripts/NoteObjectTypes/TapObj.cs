[System.Serializable]
public class TapObj : NoteObj
{
    public TapObj(uint initialKey, long initialSample)
    {
        this.initialKey = initialKey;
        this.initialSample = initialSample;
        type = NoteObjType.Tap;
    }

    public override string AsString() => 
        string.Format("{0},{1},{2}", initialKey, initialSample, (int)type);

    public override bool Empty() => base.Empty();
    public override void Clear() => base.Clear();
}
