using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RoftRecords
{
    [Serializable]
    public class Results
    {
        public Results(DateTime _timeStamp, string _grade, long _score, uint _maxCombo, uint _perfect, uint _great, uint _good, uint _ok, uint _miss)
        {
            /*TODO: With a new Result(), create a new result in the 
             * corresponding records file of a given song. 
             * Yes... More IO Stuff*/
        }
    }

    public static Results AddNewResult(DateTime _timeStamp, string _grade, long _score, uint _maxCombo, uint _perfect, uint _great, uint _good, uint _ok, uint _miss)
    {
        #region Create and return a new Result()
        return new Results(_timeStamp, _grade, _score, _maxCombo, _perfect, _great, _good, _ok, _miss);
        #endregion
    }
}
