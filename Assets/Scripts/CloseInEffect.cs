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

    public bool dispose;

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
        keyNumPosition = keyPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record)
            CloseIn();

      
    }

    private void FixedUpdate()
    {
        if (dispose && ClosestObjectClass.Instance.closestObject[keyNumPosition] != null)
        {
            ClosestObjectClass.Instance.closestObject[keyNumPosition].SetActive(false);
            ClosestObjectClass.Instance.closestObject[keyNumPosition] = null;
        } 
    }
    void CloseIn()
    {
        InHitRange();

        
        transform.localScale = new Vector3(1 / GetPercentage(), 1 / GetPercentage(), 1f);

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.2f);
        if (EditorToolClass.musicSource.timeSamples > (initiatedNoteSample + accuracy[3]) + 8000)
        {
            dispose = true;

            if (ClosestObjectClass.Instance.closestObject[keyNumPosition] == null)
                ClosestObjectClass.Instance.closestObject[keyNumPosition] = gameObject;

            BreakComboChain();
        }

        if (accuracyString != "" && Input.GetKeyDown(Key_Layout.Instance.bindedKeys[keyNumPosition]))
        {
            AudioManager.audio.Play("Normal", 50);

            IncrementComboChain();

            dispose = true;

            if (ClosestObjectClass.Instance.closestObject[keyNumPosition] == null)
            {
                ClosestObjectClass.Instance.closestObject[keyNumPosition] = gameObject;
                SendAccuracyScore();
            }
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
            bool beforePerfect = EditorToolClass.musicSource.timeSamples >= (initiatedNoteSample) - accuracy[range];
            bool afterPerfect = EditorToolClass.musicSource.timeSamples <= (initiatedNoteSample) + accuracy[range];
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
        GameObject sign = GetComponentInParent<ObjectPooler>().GetMember("Signs");
        if (!sign.activeInHierarchy)
        {
            sign.SetActive(true);
            sign.transform.position = ClosestObjectClass.Instance.closestObject[keyNumPosition].transform.position;
            sign.GetComponent<AccuracySign>().ShowSign(accuracyString);
        }
    }

    public void IncrementComboChain()
    {
        GameManager.Instance.combo++;
    }

    public void BreakComboChain()
    {
        GameManager.Instance.accuracyStats[4] += 1;
        GameManager.Instance.combo = m_break;

        GameObject sign = GetComponentInParent<ObjectPooler>().GetMember("Signs");

        if (!sign.activeInHierarchy)
        {
            sign.SetActive(true);
            sign.transform.position = gameObject.transform.position;
            sign.GetComponent<AccuracySign>().ShowSign("miss");

        }
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
        keyNum = 0; //We know what note we're on!!
        dispose = false;
    }
}