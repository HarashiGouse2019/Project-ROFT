﻿using System.Collections;
using UnityEngine;

public class CloseInEffect : NoteEffector
{
    public float initiatedNoteSample;
    public float initiatedNoteOffset;
    public float offsetStart;

    public int keyNumPosition; //So we know which key the notes are closing into
    public int keyNum; //We know what note we're on!!

    public GameObject attachedArrow;

    public SpriteRenderer sprite;

    public int index;

    public string accuracyString;

    private readonly int m_break = 0;

    public bool dispose;

    const int possibleAccuracy = 4;

    public bool dontEffectMe = false;

    //keyInputDownReceived will be given a true for one frame
    //keyInputReceived will be given so long as it's being pressed down for
    bool keyInputDownReceived, keyInputReceived;

    //So they don't have to look screwed up
    protected Color originalAppearance;
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
        keyNumPosition = keySequencePosition;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!RoftPlayer.Instance.record && GameManager.Instance.IsInteractable())
            CloseIn();

    }

    void FixedUpdate()
    {
        //Because Fixed Update goes faster than Update at a fixed rate, we're able to 
        //prevent getting multiple notes on 1 single tap.
        if (dispose && ClosestObjectClass.closestObject[keyNumPosition] != null & GameManager.Instance.IsInteractable())
        {
            if (attachedArrow != null)
            {
                attachedArrow.SetActive(false);
                attachedArrow = null;
            }

            ClosestObjectClass.closestObject[keyNumPosition].SetActive(false);
            ClosestObjectClass.closestObject[keyNumPosition] = null;
        }
    }
    
    void CloseIn()
    {
        InHitRange();

        keyInputDownReceived = Input.GetKeyDown(Key_Layout.Instance.primaryBindedKeys[keyNumPosition]) ||
                Input.GetKeyDown(Key_Layout.Instance.secondaryBindedKeys[keyNumPosition]);

        keyInputReceived = Input.GetKey(Key_Layout.Instance.primaryBindedKeys[keyNumPosition]) ||
                Input.GetKey(Key_Layout.Instance.secondaryBindedKeys[keyNumPosition]);

        if (mapReader.keys[keyNum].type == Key.KeyType.Tap)
            sprite.color = originalAppearance;

        else if (mapReader.keys[keyNum].type == Key.KeyType.Click)
            sprite.color = Color.red;

        transform.localScale = new Vector3(1 / GetPercentage(), 1 / GetPercentage(), 1f);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.15f);

        #region Auto Play
        if (CheckSoloPlay())
        {
            if (RoftPlayer.musicSource.timeSamples > (initiatedNoteSample + accuracyVal[3]))
            {
                if (ClosestObjectClass.closestObject[keyNumPosition] == null)
                    ClosestObjectClass.closestObject[keyNumPosition] = gameObject;

                BreakComboChain();

                GameManager.consecutiveMisses++;

                dispose = true;
            }

            if (accuracyString == "Perfect")
            {
                if (ClosestObjectClass.closestObject[keyNumPosition] == null)
                {
                    ClosestObjectClass.closestObject[keyNumPosition] = gameObject;

                    Pulse();
                    AudioManager.Instance.Play("Normal", 100, true);


                    GameManager.Instance.IncrementCombo();
                    SendAccuracyScore();
                    BuildStress(index);

                    GameManager.consecutiveMisses = 0;

                    dispose = true;
                }
            }
        }
        #endregion
        else
        {
            if (RoftPlayer.musicSource.timeSamples > (initiatedNoteSample + accuracyVal[3]))
            {
                if (ClosestObjectClass.closestObject[keyNumPosition] == null)
                    ClosestObjectClass.closestObject[keyNumPosition] = gameObject;

                BreakComboChain();

                GameManager.consecutiveMisses++;

                dispose = true;
            }

            
            bool tapType = (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract &&
                mapReader.keys[keyNum].type == Key.KeyType.Tap &&
                keyInputDownReceived);

            bool clickType = (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract &&
                mapReader.keys[keyNum].type == Key.KeyType.Click &&
                ClickEvent.ClickReceived());

            bool BurstType = (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract &&
                mapReader.keys[keyNum].type == Key.KeyType.Slide &&
                attachedArrow != null &&
                keyInputReceived &&
                GameManager.multiInputValue == 2);

            if (accuracyString != "" && (tapType || clickType || BurstType))
            {
                if (ClosestObjectClass.closestObject[keyNumPosition] == null)
                {
                    
                    GameManager.Instance.ResetMultiInputDelay();
                    GameManager.multiInputValue = 0;

                    if (gameObject.CompareTag("approachCircle")) ClosestObjectClass.closestObject[keyNumPosition] = gameObject;

                    if (GetPercentage() > 0.99f) targetKey = gameObject;

                    Pulse();

                    //Check type and add sound
                    if (tapType)
                        AudioManager.Instance.Play("Normal", 100, true);
                    if (BurstType)
                    {
                        if (attachedArrow.GetComponent<ArrowDirectionSet>().Detect())
                            AudioManager.Instance.Play("Ding", 100, true);
                        else return;
                    }

                    GameManager.Instance.IncrementCombo();
                    SendAccuracyScore();
                    BuildStress(index);

                    GameManager.consecutiveMisses = 0;

                    dispose = true;
                }
            }
        }
    }

    protected override float GetPercentage()
    {
        percentage = (RoftPlayer.musicSource.timeSamples - offsetStart) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    public string InHitRange()
    {
        for (int range = 0; range < accuracyVal.Length; range++)
        {
            bool beforePerfect = RoftPlayer.musicSource.timeSamples >= (initiatedNoteSample) - accuracyVal[range];
            bool afterPerfect = RoftPlayer.musicSource.timeSamples <= (initiatedNoteSample) + accuracyVal[range];

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

        GameObject sign = GetComponentInParent<ObjectPooler>().GetMember("Signs");
        if (!sign.activeInHierarchy)
        {
            sign.SetActive(true);
            sign.transform.position = ClosestObjectClass.closestObject[keyNumPosition].transform.position;
            sign.GetComponent<AccuracySign>().ShowSign(index);

            GameManager.sentScore = GameManager.accuracyScore[index];

            GameManager.Instance.accuracyStats[index] += 1;
            float inverse = ((possibleAccuracy - (index)));
            float percent = inverse / possibleAccuracy;
            GameManager.Instance.accuracyPercentile += (percent * 100);
            GameManager.Instance.overallAccuracy = (GameManager.Instance.accuracyPercentile / GameManager.Instance.GetSumOfStats());
            GameManager.Instance.UpdateScore();
        }
    }

    public void IncrementComboChain()
    {
        GameManager.Instance.IncrementCombo();

    }

    //This is unique from actually sending the accuracy score
    //There's different data to be handled and different actions to perform
    //which is why this may look like a copy-pasted SendAccuracyScore
    public void BreakComboChain()
    {
        GameManager.Instance.accuracyStats[4] += 1;
        GameManager.Instance.accuracyPercentile += (((possibleAccuracy - index) / possibleAccuracy) * 100);
        GameManager.Instance.overallAccuracy = GameManager.Instance.accuracyPercentile / GameManager.Instance.GetSumOfStats();
        GameManager.Instance.SetCombo(m_break);

        GameObject sign = GetComponentInParent<ObjectPooler>().GetMember("Signs");

        if (!sign.activeInHierarchy)
        {
            sign.SetActive(true);
            sign.transform.position = gameObject.transform.position;
            sign.GetComponent<AccuracySign>().ShowSign(4);

        }

        BuildStress(4);
    }

    public void BuildStress(int _index)
    {
        GameManager.sentStress = GameManager.stressAmount[_index] + (GameManager.Instance.stressBuild / 100);
    }

    //We want to reset all values that may
    //show itself when it reactivates.
    //We want it to be clean and refresh when we respond
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

    bool CheckSoloPlay()
    {
        return GameManager.Instance.isAutoPlaying;
    }

    void Pulse()
    {
        GameObject key;

        if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract)
        {
            key = Key_Layout.keyObjects[keyNumPosition];
            key.GetComponent<PulseEffect>().DoPulseReaction(0.15f);
            key.GetComponentInChildren<PulseEffect>().DoPulseReaction(0.15f);
            GameManager.Instance.TM_COMBO.GetComponent<PulseEffect>().DoPulseReaction(0.05f);
            GameManager.Instance.TM_COMBO_UNDERLAY.GetComponent<PulseEffect>().DoPulseReaction();
            GameManager.Instance.IMG_SCREEN_OVERLAY.GetComponent<OverlayPulseEffect>().DoPulseReaction();
        }
    }
}