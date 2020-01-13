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
        public Results(
            DateTime _timeStamp, 
            string _title, 
            string _artist, 
            string _roftID, 
            string _grade,
            long _score,
            uint _maxCombo,
            uint _perfect,
            uint _great,
            uint _good,
            uint _ok,
            uint _miss,
            float _stressBuild,
            uint _totalNotes,
            uint _keyCount,
            float _accuracyHarshness,
            float _approachSpeed)
        {
            
        }
    }

    public static Results AddNewResult(
        DateTime _timeStamp,
            string _title,
            string _artist,
            string _roftID,
            string _grade,
            long _score,
            uint _maxCombo,
            uint _perfect,
            uint _great,
            uint _good,
            uint _ok,
            uint _miss,
            float _stressBuild,
            uint _totalNotes,
            uint _keyCount,
            float _accuracyHarshness,
            float _approachSpeed)
    {
        return new Results(
            _timeStamp,
            _title,
            _artist,
            _roftID,
            _grade,
            _score,
            _maxCombo,
            _perfect,
            _great,
            _good,
            _ok,
            _miss,
            _stressBuild,
            _totalNotes,
            _keyCount,
            _accuracyHarshness,
            _approachSpeed);
    }
}
