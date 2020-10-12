using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectionNavigator : MonoBehaviour
{
    public enum NavigationState { 
        SONG_SELECTION,
        DIFFICULTY_SELECTION,
        CONFIRMATION
    }

    [SerializeField] private uint SongEntityPosition;

    [SerializeField] private uint DifficultyPosition;

    [SerializeField] private NavigationState currentNavigationState;

    public void SetSongEntityPosition(uint value)
    {
        SongEntityPosition = value;
    }

    public void SetDifficultyPosition(uint value)
    {
        DifficultyPosition = SongEntityPosition;
    }
}
