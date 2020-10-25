internal class IncrementerInt32
{
    private int Value;
    private int Multiplier = 1;
    private const int RESET = 0;
    internal IncrementerInt32(int startingValue){
        Value = startingValue;
    }

    internal IncrementerInt32(int startingValue, int multiplier)
    {
        Value = startingValue;
        Multiplier = multiplier;
    }

    /// <summary>
    /// Assign current value and increment
    /// </summary>
    internal int Next
    {
        get
        {
            return Value+=Multiplier ;
        }
    }

    /// <summary>
    /// Increment before assigning value
    /// </summary>
    internal int NextPreInc
    {
        get
        {

            return Value = (Value+=Multiplier);
        }
    }

    internal int Reset() => RESET;
}