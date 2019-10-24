using UnityEngine;

public class NoteEffect : MonoBehaviour
{
    public static NoteEffect Instance;

    #region Public Members
    [Header("Approach Speed"), Range(1.0f, 10.0f)]
    public double approachSpeed;

    [Header("Alignment")]
    public float alignment; //To shift notes whenever

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

    #endregion

    #region Private Members
    private Color alpha;

    private const int maxOffset = 100000;
    private const int minOffset = 10000;
    private const int standardAccuracy = 5;

    private float[] accuracyDefault = new float[4]
   {
        4000,
        6000,
        10000,
        12000
   }; //Default Accurary; 2-4-2 Format

    #endregion

    #region Protected Members
    protected float percentage; //Lerping for effects
    protected int keyPosition = 0; //With the collected data, what part of it are we in?
    protected float noteOffset; //When our note should start appearing
    protected int noteSample; //The note where you actually hit with timing
    #endregion

    private void Awake()
    {
        Instance = this;
        UpdateAccuracyHarshness();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNoteOffset();
        UpdateAccuracyHarshness();

        if (!EditorToolClass.Instance.record && CheckMusicSample())
            Approach();
    }

    void Approach()
    {
        CloseInEffect effect;

        if (keyPosition < mapReader.keys.Count)
        {
            //GameObject approachCircle = Instantiate(notePrefab, Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform);
            ObjectPooler keyPooler = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].GetComponent<ObjectPooler>();
            GameObject approachCircle = keyPooler.GetMember("Approach Circle");

            effect = approachCircle.GetComponent<CloseInEffect>();

            if (!approachCircle.activeInHierarchy)
            {
                approachCircle.SetActive(true);
                approachCircle.transform.position = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.position;
                approachCircle.transform.localScale = Key_Layout.keyObjects[mapReader.keys[keyPosition].keyNum].transform.localScale;
                
            }
            
            effect.initiatedNoteSample = noteSample;
            effect.initiatedNoteOffset = noteOffset;
            effect.offsetStart = noteSample - noteOffset;
            effect.accuracyVal = accuracyVal;
            effect.keyNum = keyPosition;
            effect.keyNumPosition = mapReader.keys[effect.keyNum].keyNum;



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

    void UpdateNoteOffset()
    {
        noteOffset = maxOffset - (minOffset * ((float)approachSpeed - 1));
    }

    void UpdateAccuracyHarshness()
    {
        for (int accuracyScoreIndex = 0; accuracyScoreIndex < accuracyVal.Length; accuracyScoreIndex++)
        {
            accuracyVal[accuracyScoreIndex] = accuracyDefault[accuracyScoreIndex] + (500 * (standardAccuracy - accuracy));
        }
    }

    protected virtual float GetPercentage()
    {
        return 0;
    }
}
