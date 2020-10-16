using System;

public class MapErrorException : Exception
{
    public MapErrorException(string message) : base(message) { GameManager.ErrorDetected = true; }
}
