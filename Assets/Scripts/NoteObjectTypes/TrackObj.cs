using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class TrackObj : NoteObj
{
    //Misscellaneous data for Track Type
    protected List<TrackPoint> Points;

    

    public TrackObj(uint initialKey, long initialSample, List<TrackPoint> points) : base()
    {
        InitialKey = initialKey;
        InitialSample = initialSample;
        Points = points;
        Type = NoteObjType.Track;
    }

    public override string AsString() => $"{InitialKey},{InitialSample},{(int)Type},{"{"}{PointListToRoftFormatString()}{"}"}";
    
    string PointListToRoftFormatString()
    {
        string formatString = string.Empty;
        StringBuilder sb = new StringBuilder(formatString);
        for(int index = 0; index < Points.Count; index++)
        {
            TrackPoint point = Points[index];
            sb.Append(string.Format(index < Points.Count - 1 ? "{0},{1}|" : "{0},{1}", point.connectionKey, point.connectionSample));
        }

        return formatString;
    }

    public override bool Empty() =>
        InitialKey == 0 && InitialSample == 0 && Type == default && (Points == null || Points.Count == 0);

    public override void Clear()
    {
        base.Clear();
        Points.Clear();
    }

    public void SetTrackPoint(List<TrackPoint> pointValues) => Points = pointValues;
    public List<TrackPoint> GetTrackPoints() => Points;
}
