[System.Serializable]
public class HoldObj : NoteObj
{
    //Miscellaneous data for Hold Type
    protected long finalSample = -1;

    

    public HoldObj(uint initialKey, long initialSample, long finalSample)
    {
        this.initialSample = initialKey;
        this.initialSample = initialSample;
        this.finalSample = finalSample;
        type = NoteObjType.Hold;
    }

    public override string AsString() => 
        string.Format("{0},{1},{2},{3}", initialKey, initialSample, (int)type, finalSample);

    public override bool Empty() =>
        (initialKey == 0 && initialSample == 0 && type == default && finalSample == 0);

    public override void Clear()
    {
        initialKey = 0;
        initialSample = 0;
        type = default;
        finalSample = 0;
    }

    public void SetFinalSample(long value) => finalSample = value; 
    public long GetFinalSample() => finalSample;
}
