[System.Serializable]
public class BurstObj : NoteObj
{

    public BurstObj(uint initialKey, long initialSample, uint direction)
    {
        this.initialKey = initialKey;
        this.initialSample = initialSample;
        this.direction = direction;
        type = NoteObjType.Burst;
    }

    public override string AsString() =>
         string.Format("{0},{1},{2},{3}", initialKey, initialSample, (int) type, direction);

    public void SetDirection(uint directionValue) => direction = directionValue;
    public uint GetDirecetion() => direction;
}
