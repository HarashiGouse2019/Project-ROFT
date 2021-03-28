using System;

public class SongNotFoundException : Exception
{
    public SongNotFoundException(string message = "Songs have not been found during Scouting...")
        : base(message)
    {
        ErrorHandler.PushError(new ErrorHandler.ErrorLog(this));
    }
}
