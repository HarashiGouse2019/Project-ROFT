using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEffector : MonoBehaviour
{
    public static NoteEffector Instance;

    #region Public Members
    [Header("Approach Speed"), Range(1.0f, 10.0f)]
    public double approachSpeed;

    [Header("Alignment")]
    public float alignment; //To shift notes whenever

    //When TypeByRegion is on,
    //The keys will need to be off by the
    //actual approach circle.
    [Header("KeyAppearOffset"), Range(1.0f, 10.0f)]
    public float keyAppearOffset = 1f;


    /*
     * Perfect - 0
     * Great - 1
     * Good - 2
     * Ok - 3
    */
    [Header("Accuracy"), Range(1f, 10f)]
    public float accuracy = 5f; //5 is the average/standard accuracy in the game.

    public float[] accuracyVal = new float[4];

    public MapReader mapReader;

    //We get our images
    [Header("UI ASSET")]
    public GameObject notePrefab;

    public static int keySequencePosition = 0; //With the collected data, what part of it are we in?

    #endregion

    #region Private Members
    private Color alpha;

    private const int maxOffset = 100000;
    private const int minOffset = 10000;
    private const int standardAccuracy = 5;

    readonly private float[] accuracyDefault = new float[4]
    {
        4000,
        6000,
        10000,
        12000
    }; //Default Accurary; 2-4-2 Format

    private RoftIO mainIO;
    #endregion

    #region Protected Members
    protected float percentage; //Lerping for effects
    protected int keyObjPosition = 0;
    protected float noteOffset; //When our note should start appearing
    protected int noteSample; //The note where you actually hit with timing
    protected int noteSampleForKey; //The time the key is at full capacity (for TBR Modes)
    protected KeyCode randomKey;
    protected GameObject lastKeySpawned;
    protected List<GameObject> spawnedKeysHistory = new List<GameObject>();
    protected GameObject targetKey;
    #endregion

    private void Awake()
    {
        Instance = this;
        mainIO = GameManager.GiveAccessToIO();
        RetrieveEffectConfigs();
        UpdateAccuracyHarshness();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNoteOffset();
        UpdateAccuracyHarshness();

        if (!RoftPlayer.Instance.record && ApproachCircleSpawnTime())
            Approach();
    }

    void Approach()
    {
        //We have two effects for the approach circle, and for the arrows
        CloseInEffect effect = null;
        CloseInEffect arrowEffect = null;

        if (keySequencePosition < mapReader.noteObjs.Count)
        {
            ObjectPooler keyPooler;

            #region HELP?
            //This grabs our note objects from the "key object" based on what
            //note instance is occuring, as well as if it's 
            //the first "key" on the screen, or the third.
            //Most of this information is from the MapReader, in which that object
            //reads from the song file. 
            #endregion
            keyPooler = Key_Layout.keyObjects[mapReader.noteObjs[keySequencePosition].instID].GetComponent<ObjectPooler>();

            //We want to get our game objects from the same key for both the approach circle, and the arrow
            GameObject approachCircle = keyPooler.GetMember("Approach Circle");
            GameObject arrowDirection = keyPooler.GetMember("Arrows");

            effect = approachCircle.GetComponent<CloseInEffect>();

            //We don't want arrows to show or do an effect, until we know that's the type we're dealing with
            if (mapReader.noteObjs[keySequencePosition].instType == NoteObj.NoteObjType.Burst)
                arrowEffect = arrowDirection.GetComponent<CloseInEffect>();

            //Check if Approach Cirlce is not active
            if (!approachCircle.activeInHierarchy)
            {
                WakeUpNoteMember(approachCircle);
                AssignPosition(effect);
            }

            //If in Techmeister and there's a sliding type, check if Arrow is not active
            if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract &&
                mapReader.noteObjs[keySequencePosition].instType == NoteObj.NoteObjType.Burst &&
                !arrowDirection.activeInHierarchy)
            {
                WakeUpNoteMember(arrowDirection);

                arrowDirection.GetComponent<ArrowDirectionSet>().SetDirection(mapReader.noteObjs[keySequencePosition].miscellaneousValue1);

                effect.attachedArrow = arrowDirection;

                arrowDirection.GetComponent<ArrowDirectionSet>().AttachedCircle = approachCircle;

                AssignPosition(arrowEffect);
            }

            UpdateToNextNote();
        }
    }

    private void WakeUpNoteMember(GameObject _obj)
    {
        _obj.SetActive(true);
        _obj.transform.position = Key_Layout.keyObjects[mapReader.noteObjs[keySequencePosition].instID].transform.position;
        _obj.transform.localScale = Key_Layout.keyObjects[mapReader.noteObjs[keySequencePosition].instID].transform.localScale;
    }

    private void UpdateToNextNote()
    {
        keySequencePosition++;
    }

    //There's two objects;
    //Approach Circle, and Key
    bool ApproachCircleSpawnTime()
    {
        if (keySequencePosition < mapReader.noteObjs.Count)
        {
            noteSample = mapReader.noteObjs[keySequencePosition].instSample - (int)alignment;
            float offsetStart = noteSample - noteOffset;

            //This is strictly for checking when notes should appear
            if (RoftPlayer.musicSource.timeSamples > offsetStart)
                return true;
        }
        return false;
    }

    void UpdateNoteOffset()
    {
        noteOffset = maxOffset - (minOffset * (((float)approachSpeed / 1.10f) - 1));
    }

    void UpdateAccuracyHarshness()
    {
        for (int accuracyScoreIndex = 0; accuracyScoreIndex < accuracyVal.Length; accuracyScoreIndex++)
        {
            accuracyVal[accuracyScoreIndex] = accuracyDefault[accuracyScoreIndex] + (500 * (standardAccuracy - accuracy));
        }
    }

    void AssignPosition(CloseInEffect _effect)
    {
        _effect.initiatedNoteSample = noteSample;
        _effect.initiatedNoteOffset = noteOffset;
        _effect.offsetStart = noteSample - noteOffset;
        _effect.accuracyVal = accuracyVal;

        //This is referring to list index in the MapReader
        _effect.keyNum = keySequencePosition;

        //This is the index regarding each "key" on screen
        _effect.keyNumPosition = mapReader.noteObjs[_effect.keyNum].instID;
    }

    protected virtual float GetPercentage()
    {
        return 0;
    }

    void RetrieveEffectConfigs()
    {
        //Set Up values for NoteEffect
        int difficultyTag = mainIO.InRFTMJumpTo("Difficulty", mapReader.m_name);
        accuracy = mainIO.ReadPropertyFrom<float>(difficultyTag, "AccuracyHarshness", mapReader.m_name);
        approachSpeed = mainIO.ReadPropertyFrom<float>(difficultyTag, "ApproachSpeed", mapReader.m_name);
    }
}
