using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEffect : MonoBehaviour
{
    public static NoteEffect Instance;

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

    public static int keyPosition = 0; //With the collected data, what part of it are we in?

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
    #endregion

    private void Awake()
    {
        Instance = this;
        //Set Up values for NoteEffect
        int difficultyTag = MapReader.Instance.InRFTMJumpTo("Difficulty");
        accuracy = float.Parse(MapReader.Instance.ReadPropertyFrom(difficultyTag, "AccuracyHarshness"));
        approachSpeed = float.Parse(MapReader.Instance.ReadPropertyFrom(difficultyTag, "ApproachSpeed"));
        UpdateAccuracyHarshness();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNoteOffset();
        UpdateAccuracyHarshness();

        switch (Key_Layout.Instance.layoutMethod)
        {
            case Key_Layout.LayoutMethod.Abstract:
                if (!EditorToolClass.Instance.record && ApproachCircleSpawnTime())
                    Approach();
                break;

            case Key_Layout.LayoutMethod.Region_Scatter:

                if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter
                    && !EditorToolClass.Instance.record
                    && KeySpawnTime())
                {
                    randomKey = Key_Layout.Instance.RandomizeAndProcess();
                    Vector2 spawnTarget = new Vector2(
                        Key_Layout.Instance.spawnPositionX,
                        Key_Layout.Instance.spawnPositionY
                        );

                    Appear(spawnTarget);

                    //Now show approach circle
                    if (ApproachCircleSpawnTime())
                        Approach();
                }
                break;
        }
    }

    void Approach()
    {
        //We have two effects for the approach circle, and for the arrows
        CloseInEffect effect = null;
        CloseInEffect arrowEffect = null;

        if (keyPosition < mapReader.keys.Count)
        {
            ObjectPooler keyPooler;
            ObjectPooler keyLayoutPooler = Key_Layout.Instance.GetComponent<ObjectPooler>();

            if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract)
                keyPooler = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].GetComponent<ObjectPooler>();
            else
                keyPooler = keyLayoutPooler.pooledObjects[keyLayoutPooler.poolIndex].GetComponent<ObjectPooler>();

            //We want to get our game objects from the same key for both the approach circle, and the arrow
            GameObject approachCircle = keyPooler.GetMember("Approach Circle");
            GameObject arrowDirection = keyPooler.GetMember("Arrows");

            effect = approachCircle.GetComponent<CloseInEffect>();

            //We don't want arrows to show or do an effect, until we know that's the type we're dealing with
            if (mapReader.keys[keyPosition].type == Key.KeyType.Slide)
                arrowEffect = arrowDirection.GetComponent<CloseInEffect>();

            //Check if Approach Cirlce is not active
            if (!approachCircle.activeInHierarchy)
            {
                approachCircle.SetActive(true);
                if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract)
                {
                    approachCircle.transform.position = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.position;
                    approachCircle.transform.localScale = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.localScale;
                }
                else
                {
                    approachCircle.transform.position = keyLayoutPooler.pooledObjects[keyLayoutPooler.poolIndex].transform.position;
                    approachCircle.transform.localScale = keyLayoutPooler.pooledObjects[keyLayoutPooler.poolIndex].transform.localScale;

                    AppearEffect targetEffect = keyLayoutPooler.pooledObjects[keyLayoutPooler.poolIndex].GetComponent<AppearEffect>();
                    targetEffect.assignedCircle = approachCircle;
                }
                AssignPosition(effect);
            }

            //If in Techmeister and there's a sliding type, check if Arrow is not active
            if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Abstract &&
                mapReader.keys[keyPosition].type == Key.KeyType.Slide &&
                !arrowDirection.activeInHierarchy)
            {
                arrowDirection.SetActive(true);
                arrowDirection.transform.position = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.position;
                arrowDirection.transform.localScale = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.localScale;
                arrowDirection.GetComponent<ArrowDirectionSet>().SetDirection(mapReader.keys[keyPosition].miscellaneousValue1);
                effect.attachedArrow = arrowDirection;
                arrowDirection.GetComponent<ArrowDirectionSet>().attachedCircle = approachCircle;
                AssignPosition(arrowEffect);
            }

            keyPosition++;
        }
    }

    //This is for the key's to appear on screen
    //It'll take the randomized number from using
    //KeyLayout's function RandomizeAndSend()
    void Appear(Vector2 _targetPosition)
    {
        AppearEffect effect;
        if (keyObjPosition < mapReader.keys.Count)
        {
            GameObject keyMember = Key_Layout.Instance.pooler.GetMember("keys");
            effect = keyMember.GetComponent<AppearEffect>();



            Vector3 screenPosition = (Vector3)_targetPosition + new Vector3(0f, 0f, Camera.main.nearClipPlane);

            if (!keyMember.activeInHierarchy)
            {
                keyMember.SetActive(true);
                effect.enabled = true;
                keyMember.transform.localPosition = Key_Layout.Instance.m_camera.ScreenToWorldPoint(screenPosition);
                keyMember.transform.rotation = Quaternion.identity;

                if (spawnedKeysHistory.Count > 2)
                    CompareWithPreviousKey(keyMember);

                spawnedKeysHistory.Add(keyMember);
            }

            effect.initiatedNoteSample = noteSampleForKey;
            effect.offsetStart = noteSampleForKey - noteOffset;
            effect.keyNum = Key_Layout.Instance.pooler.poolIndex;
            effect.assignedKeyBind = randomKey;
        }
    }

    void CompareWithPreviousKey(GameObject _currentKey)
    {
        Transform currentKeyTransform = _currentKey.GetComponent<Transform>();
        Transform spawnedKeyTransform = null;

        //Find closest

        for (int keyIndex = 0; keyIndex < Key_Layout.Instance.pooler.pooledObjects.Count; keyIndex++)
        {
            if (spawnedKeyTransform == null && Vector3.Distance(currentKeyTransform.position, Key_Layout.Instance.pooler.pooledObjects[keyIndex].transform.position) < 5f)
                spawnedKeyTransform = Key_Layout.Instance.pooler.pooledObjects[keyIndex].transform;
        }

        bool greaterThanX = currentKeyTransform.position.x > spawnedKeyTransform.position.x - Key_Layout.padding;
        bool greaterThanY = currentKeyTransform.position.y > spawnedKeyTransform.position.y - Key_Layout.padding;
        bool lessThanX = currentKeyTransform.position.x < spawnedKeyTransform.position.x + Key_Layout.padding;
        bool lessThanY = currentKeyTransform.position.y < spawnedKeyTransform.position.y + Key_Layout.padding;

        float currentXPos = currentKeyTransform.position.x;
        float currentYPos = currentKeyTransform.position.y;
        float currentZPos = currentKeyTransform.position.z;


        //These ifs are trash. _-_
        if (greaterThanX)
            currentKeyTransform.position = new Vector3(currentXPos - Key_Layout.padding / 50, currentYPos, currentZPos);

        if (greaterThanY)
            currentKeyTransform.position = new Vector3(currentXPos, currentYPos - Key_Layout.padding / 50, currentZPos);

        if (lessThanX)
            currentKeyTransform.position = new Vector3(currentXPos + Key_Layout.padding / 50, currentYPos, currentZPos);

        if (lessThanY)
            currentKeyTransform.position = new Vector3(currentXPos, currentYPos + Key_Layout.padding / 50, currentZPos);
    }

    //There's two objects;
    //Approach Circle, and Key
    bool ApproachCircleSpawnTime()
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

    bool KeySpawnTime()
    {
        if (keyPosition < mapReader.keys.Count)
        {
            noteSampleForKey = mapReader.keys[keyPosition].keySample - (int)alignment - (int)(keyAppearOffset);
            float offsetStart = noteSampleForKey - noteOffset;

            //This is strictly for checking when notes should appear
            if (EditorToolClass.musicSource.timeSamples > offsetStart)
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
        _effect.keyNum = keyPosition;
        _effect.keyNumPosition = mapReader.keys[_effect.keyNum].keyNum;
    }

    protected virtual float GetPercentage()
    {
        return 0;
    }
}
