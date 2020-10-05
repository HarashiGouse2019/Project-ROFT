using UnityEngine;

using static ROFTIOMANAGEMENT.RoftIO;

public class InteractableKey : MonoBehaviour
{
    SpriteRenderer sprite;
    Color originalColor;
    int keyNum;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    private void OnMouseOver()
    {
        sprite.color = Color.green;
        //Collect Data
        string[] data = {
            string.Format("{0}, {1}, {2}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString()),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.Instance.finishSample),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.Instance.trackPoints),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.Instance.burstDirection)
        };
        Key_Layout.Instance.dataText.text = data[(int)ObjectLogger.noteTool];
        
    }

    private void OnMouseDown()
    {
        #region Write to RFTM File

        /*If we happen to be recording, and we hit the second set of binded keys, the
                data will be written to a file.*/
        //if (RoftPlayer.Instance.record)
        //{
        //    string data =
        //        keyNum.ToString() + ","
        //         + RoftPlayer.musicSource.timeSamples.ToString() + ","
        //        + ((int)ObjectLogger.noteTool).ToString();

        //    WriteNewObjectToRFTM(RoftCreator.filename, RoftCreator.newSongDirectoryPath + "/", data);
        //}
        #endregion

        //Check the type, then note the object data from ObjectLogger
        if (RoftPlayer.Instance.record)
        {
            if(ObjectLogger.noteTool == ObjectLogger.NoteTool.TAP)
                ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
            else if (ObjectLogger.noteTool == ObjectLogger.NoteTool.HOLD)
                ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
            else if (ObjectLogger.noteTool == ObjectLogger.NoteTool.TRACK)
                ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
            else if (ObjectLogger.noteTool == ObjectLogger.NoteTool.BURST)
                ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool, ObjectLogger.Instance.burstDirection);
        }
    }

    private void OnMouseExit()
    {
        sprite.color = originalColor;
    }

    public void SetKeyNum(int _value)
    {
        keyNum = _value;
    }
}
