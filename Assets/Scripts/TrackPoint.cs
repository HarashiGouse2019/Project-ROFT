public sealed class TrackPoint
{
    public uint connectionKey;
    public long connectionSample;
    public TrackPoint(uint connectionKeyValue, long connectionSampleValue)
    {
        connectionKey = connectionKeyValue;
        connectionSample = connectionSampleValue;
    }
}