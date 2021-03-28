using UnityEngine;

public class SongSelectionNavigator : MonoBehaviour
{
    private uint SongEntityPosition;

    private uint DifficultyPosition;

    public void Begin()
    {
        MapReader.Read((int)SongEntityPosition, (int)DifficultyPosition);
    }

    public void BeginEdit()
    {

    }

    public void SetSongEntityPosition(uint value)
    {
        SongEntityPosition = value;
    }

    public void SetDifficultyPosition(int value)
    {
        DifficultyPosition = (uint)value;
    }
}
