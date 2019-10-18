using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEffect : MonoBehaviour
{
    #region Public Members
    public int noteOffset; //When our note should start appearing
    public int noteSample; //The note where you actually hit with timing
    public float alignment; //To shift notes whenever
    public MapReader mapReader;

    //We get our images
    [Header("UI ASSET")]
    public GameObject notePrefab;

    #endregion

    #region Private Members
    protected float percentage; //Lerping for effects
    protected int keyPosition = 0; //With the collected data, what part of it are we in?
    private Color alpha;

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record && CheckMusicSample())
        {
            Approach();
            
        }    
    }

    void Approach()
    {
        CloseInEffect effect;
        if (keyPosition < mapReader.keys.Count)
        {
            GameObject approachCircle = Instantiate(notePrefab, Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform);

            effect = approachCircle.GetComponent<CloseInEffect>();
            effect.initiatedNoteSample = noteSample;
            effect.initiatedNoteOffset = noteOffset;
            effect.offsetStart = noteSample - noteOffset;

            keyPosition++;
        }
    }

    bool CheckMusicSample()
    {
        if (keyPosition < mapReader.keys.Count)
        {
            noteSample = mapReader.keys[keyPosition].keySample - (int)alignment;
            float offsetStart = noteSample - noteOffset;

            //This is strictly for checking when notes should appear
            if (EditorToolClass.musicSource.timeSamples > offsetStart)
                return true;
        }
        return false;
    }

    protected virtual float GetPercentage()
    {
        return 0;
    }
}
