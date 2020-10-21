using UnityEngine;
using UnityEngine.UI;
using Extensions;

public class InteractableKey : MonoBehaviour
{
    Image sprite;
    Color originalColor;
    int keyNum;
    bool marked;
    private void Awake()
    {
        sprite = GetComponent<Image>();
        originalColor = sprite.color;
    }

    private void OnMouseOver()
    {
        sprite.color = Color.green;
        //Collect Data
        Var<string[]> data = new[] {
            string.Format("{0}, {1}, {2}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString()),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.GetFinishSample()),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.GetTrackPoints()),
            string.Format("{0}, {1}, {2}, {3}", keyNum, RoftPlayer.musicSource.timeSamples, ((int)ObjectLogger.noteTool).ToString(), ObjectLogger.GetBurstDirection())
        };
        Key_Layout.Instance.dataText.text = data.Value[(int)ObjectLogger.noteTool];
        
    }

    private void OnMouseDown()
    {

        //Check the type, then note the object data from ObjectLogger
        if (RoftPlayer.Record)
        {
            switch (ObjectLogger.noteTool)
            {
                case ObjectLogger.NoteTool.TAP:
                    ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
                    return;
                case ObjectLogger.NoteTool.HOLD:
                    ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
                    return;
                case ObjectLogger.NoteTool.TRACK:
                    ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool);
                    return;
                case ObjectLogger.NoteTool.BURST:
                    ObjectLogger.LogInNoteObject(ObjectLogger.noteTool, keyNum, (long)RoftPlayer.musicSource.timeSamples, (int)ObjectLogger.noteTool, ObjectLogger.GetBurstDirection());
                    return;
                default:
                    return;
            }
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

    public void Mark() => marked = true;
    public void UnMark() => marked = false;
}
