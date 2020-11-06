[System.Serializable]
public class BurstObj : NoteObj
{
    //Misscellaneous data for Burst Type
    protected uint Direction;

    public BurstObj(uint initialKey, long initialSample, uint direction) : base()
    {
        InitialKey = initialKey;
        InitialSample = initialSample;
        Direction = direction;

        Type = NoteObjType.Burst;
    }

    public override string AsString() => $"{InitialKey},{InitialSample},{(int)Type},{PatternSetValue},{TickValue},{LayerValue}";

    public void SetDirection(uint directionValue) => Direction = directionValue;
    public uint GetDirecetion() => Direction;
    public override void Clear()
    {
        base.Clear();
        Direction = 0;
    }
}
