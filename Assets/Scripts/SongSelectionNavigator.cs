﻿using UnityEngine;
using System;

public class SongSelectionNavigator : MonoBehaviour
{
    [SerializeField] private uint SongEntityPosition;

    [SerializeField] private uint DifficultyPosition;

    public void Begin()
    {

        MapReader.Read((int)SongEntityPosition, (int)DifficultyPosition);

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
