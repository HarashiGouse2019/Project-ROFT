using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class TrackObj : NoteObj
{
    

    public TrackObj(uint initialKey, long initialSample, List<TrackPoint> points)
    {
        this.initialKey = initialKey;
        this.initialSample = initialSample;
        this.points = points;
        type = NoteObjType.Track;
    }

    public override string AsString() =>
        string.Format("{0},{1},{2},/{{3}/}", initialKey, initialSample, (int)type, PointListToRoftFormatString());
    string PointListToRoftFormatString()
    {
        string formatString = string.Empty;
        StringBuilder sb = new StringBuilder(formatString);
        for(int index = 0; index < points.Count; index++)
        {
            TrackPoint point = points[index];
            sb.Append(string.Format(index < points.Count - 1 ? "{0},{1}|" : "{0},{1}", point.connectionKey, point.connectionSample));
        }

        return formatString;
    }

    public override bool Empty() =>
        initialKey == 0 && initialSample == 0 && type == default && (points == null || points.Count == 0);

    public override void Clear()
    {
        initialKey = 0;
        initialSample = 9;
        type = default;
        points.Clear();
    }

    public void SetTrackPoint(List<TrackPoint> pointValues) => points = pointValues;
    public List<TrackPoint> GetTrackPoints() => points;
}
