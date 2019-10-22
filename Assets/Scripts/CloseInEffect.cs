using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseInEffect : NoteEffect
{
    public float initiatedNoteSample;
    public float initiatedNoteOffset;
    public float offsetStart;

    public int keyNumPosition; //So we know which key the notes are closing into
    public int keyNum; //We know what note we're on!!

    public SpriteRenderer sprite;

    public int index;

    public string accuracyString;

    private readonly int m_break = 0;

    //So they don't have to look screwed up
    Color originalAppearance;

    private void Awake()
    {
        mapReader = MapReader.Instance;
        sprite = GetComponent<SpriteRenderer>();
        originalAppearance = sprite.color;
    }

    //Start
    private void OnEnable()
    {
        initiatedNoteSample = noteSample;
        initiatedNoteOffset = noteOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record)
        {
            InHitRange();
            CloseIn();
        }
    }

    void CloseIn()
    {
        transform.localScale = new Vector3(1 / GetPercentage(), 1 / GetPercentage(), 1f);

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.2f);

        if (EditorToolClass.musicSource.timeSamples > (initiatedNoteSample + accuracy[3]) + 8000)
        {
            BreakComboChain();
            gameObject.SetActive(false);
        }

        if (accuracyString != "" && Input.GetKeyDown(Key_Layout.Instance.bindedKeys[keyNumPosition]))
        {
            //I believe we have to find which one of the notes are the closes
            //We would have to find the difference between all the notes that are in range, and get the closes one
            //I have no idea how I'm going to do that. I just know that's something that we have to consider.....
            AudioManager.audio.Play("Normal", 50);
            IncrementComboChain();
            SendAccuracyScore();


            gameObject.SetActive(false);
        }
    }

    protected override float GetPercentage()
    {
        percentage = (EditorToolClass.musicSource.timeSamples - offsetStart) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    public string InHitRange()
    {
        for (int range = 0; range < accuracy.Length; range++)
        {
            bool beforePerfect = EditorToolClass.musicSource.timeSamples >= initiatedNoteSample - accuracy[range] * 2;
            bool afterPerfect = EditorToolClass.musicSource.timeSamples <= (initiatedNoteSample + accuracy[range]) * 2;
            if (beforePerfect && afterPerfect)
            {
                accuracyString = GameManager.accuracyString[range];
                index = range;

                return accuracyString;
            }
        }
        return null;
    }

    public void SendAccuracyScore()
    {
        GameManager.Instance.accuracyStats[index] += 1;
        GameManager.sentScore = GameManager.accuracyScore[index];
    }

    public void IncrementComboChain()
    {
        GameManager.Instance.combo++;
    }

    public void BreakComboChain()
    {
        GameManager.Instance.accuracyStats[4] += 1;
        GameManager.Instance.combo = m_break;
    }

    private void OnDisable()
    {
        sprite.color = originalAppearance;
        percentage = 0;
        index = 0;
        initiatedNoteSample = 0;
        initiatedNoteOffset = 0;
        offsetStart = 0;
        accuracyString = "";
        keyNumPosition = 0; //So we know which key the notes are closing into
        keyNum = 0; //We know what note we're on!!
    }
}