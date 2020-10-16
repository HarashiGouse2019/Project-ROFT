using System;

public class MapErrorException : Exception
{
    public MapErrorException(string message) : base(message) {
        ErrorHandler.PushError(new ErrorHandler.ErrorLog(this));  
    }
}
