using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ROFTIOMANAGEMENT;
using System;
using System.Linq.Expressions;

public class ObjectLogger : MonoBehaviour
{
    public static ObjectLogger Instance;

    public class TrackPoint
    {
        public int initialKey;
        public long initialSample;
    }

    [Serializable]
    public class PatternSet
    {
        //We represent a NoteObj at this cell with a given
        //Tick value and Pattern value
        public int ID = 1;

        NoteObj[] logObject;
        public void Init(int size)
        {
            logObject = new NoteObj[size];
        }

        public void LogObject(NoteObj obj, int position)
        {
            logObject[position] = obj;
        }

        public int Size() => logObject.Length;

        public bool IsCellEmpty(int value)
        {
            try
            {
                return logObject != null && logObject[value].Empty();
            }
            catch { return true; }
        }
    }

    public TextMeshProUGUI TMP_Tick;
    public TextMeshProUGUI TMP_PatternSet;
    public enum LoggerSize
    {
        WALTZ = 3,
        WHOLE = 4,
        SIXTH = 6,
        EIGHTH = 8,
        SIXTEEN = 16,
        THRITY_SECNOND = 32,
        SIXTY_FORTH = 64,
        ONE_HUNTRED_TWENTY_EIGHTH = 128
    }

    public enum NoteTool
    {
        TAP,
        HOLD,
        TRACK,
        BURST
    }

    public LoggerSize loggerSize = LoggerSize.WHOLE;

    [Range(1f, 4f)]
    public float rate = 1f;

    public float offsetInSeconds = 0;

    public GameObject prefab;

    readonly Color c_currentTick = Color.white;
    readonly Color c_emptyTick = new Color(25f / MAX_ALPHA, 25f / MAX_ALPHA, 25f / MAX_ALPHA);
    readonly Color c_emptyTickGrey = Color.grey;
    readonly Color c_emptyTickDarkGrey = new Color(50f / MAX_ALPHA, 50f / MAX_ALPHA, 50f / MAX_ALPHA);
    readonly Color c_tickNoteData = new Color(16f / MAX_ALPHA, 210f / MAX_ALPHA, 213f / MAX_ALPHA);
    readonly Color c_tickTimeData = new Color(247f / MAX_ALPHA, 244f / MAX_ALPHA, 40f / MAX_ALPHA);

    [SerializeField]
    public float bpm = 120;
    float oldBPM = 0;

    GameObject[] blocks;

    RectTransform rectTransform;

    const float MINUTEINSEC = 60;

    const float MAX_ALPHA = 255f;

    float tick = -1;
    float inGameTime = 0;


    List<NoteObj> objects = new List<NoteObj>();

    List<PatternSet> patternSets = new List<PatternSet>();

    const float reset = 0f;

    //Data
    public int keyData;
    public long sampleData;
    public int typeData;

    //Unique data
    public long finishSample; //For hold type
    public List<TrackPoint> trackPoints; //For track type
    public int burstDirection; //For burst type

    string dataFormat;

    public static NoteTool noteTool;
    float tickValue;
    float currentTick;
    float currentPatternSet;

    bool sequenceDone = false;

    private void Awake()
    {
        Instance = this;

    }
    // Start is called before the first frame update
    void Start()
    {
        Init((int)loggerSize);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inGameTime = RoftPlayer.musicSource.time - offsetInSeconds;

        tickValue = inGameTime / (60f / (bpm * rate * ((float)loggerSize / (float)LoggerSize.WHOLE)));
        currentTick = Mathf.Floor(tickValue);
        currentPatternSet = Mathf.Floor(tickValue / (float)loggerSize);

        if (tick != currentTick && inGameTime > 0f)
        {
            tick = currentTick;
            TMP_Tick.text = ((tick % (int)loggerSize) + 1).ToString();
            TMP_PatternSet.text = (currentPatternSet + 1).ToString();
            if (Mathf.Floor(inGameTime % (60f / bpm)) == 0)
                Tick();
        }
    }


    void Init(int size)
    {
        #region BlockUI Setup
        blocks = new GameObject[size];
        float width = 0;
        for (int iter = 0; iter < size; iter++)
        {
            blocks[iter] = Instantiate(prefab, transform);
            blocks[iter].transform.position = transform.position + new Vector3(1.05f * iter, 0f, 0f);
            blocks[iter].transform.rotation = Quaternion.identity;
            blocks[iter].transform.localScale = Vector3.one;
            width += blocks[iter].GetComponent<RectTransform>().rect.width + 1.05f;

            blocks[iter].GetComponent<Button>().image.color = iter == 0 ? c_currentTick :
                (int)loggerSize != 4 && iter % ((int)loggerSize / 8) != 0 ? c_emptyTickGrey :
                iter % ((int)loggerSize / 4) != 0 ? c_emptyTickDarkGrey :
                c_emptyTick;
        }
        #endregion

        #region PatternSets Initialization
        //With the given information from BPM, we have to see how many times we'll go over each measure
        //starting when the song begins (or where our offset is)

        //This already gives us how many seconds it takes to go between each beat.
        var tickValue = inGameTime / (60f / (bpm * rate * ((float)loggerSize / (float)LoggerSize.WHOLE)));

        //In order to achieve how many pattern set's we need, we need to take the sample rate of the some
        //get the song length in samples, and iterate as much as we can until we're beyond the song length
        //The number we get is the possible amount of PatternSet we can have when creating the Songmap.
        var sampleRate = 1f;
        var songLength = RoftPlayer.musicSource.clip.length - offsetInSeconds;
        var trackPosition = 0f;

        var totalPatternSets = 0f;
        while (trackPosition < songLength)
        {
            trackPosition += sampleRate;

            var samplesPerBeat = trackPosition / (60f / (bpm * rate * ((float)loggerSize / (float)LoggerSize.WHOLE)));

            totalPatternSets = Mathf.Floor(samplesPerBeat / (float)loggerSize);
        }

        for (int i = 0; i < totalPatternSets; i++)
        {
            PatternSet newSet = new PatternSet();
            newSet.Init((int)loggerSize);
            patternSets.Add(newSet);
        }
        #endregion
    }

    void Tick()
    {
        for (int iter = 0; iter < blocks.Length; iter++)
        {
            if (tick % (int)loggerSize == iter)
            {
                blocks[iter].GetComponent<Button>().image.color = c_currentTick;
            }
            else
            {
                blocks[iter].GetComponent<Button>().image.color = (int)loggerSize != 4 && iter % ((int)loggerSize / 8) != 0 ? c_emptyTickGrey :
                iter % ((int)loggerSize / 4) != 0 ? c_emptyTickDarkGrey :
                c_emptyTick;
                
            }
        }

        ShowMarksInPatternSet();
    }

    void AddObject(NoteObj newObject)
    {
        objects.Add(newObject);
        RefreshLog();
    }

    void RemoveObject(NoteObj objectTarget)
    {
        objects.Remove(objectTarget);
        RefreshLog();
    }

    void RefreshLog()
    {
        var _objects = from o in objects orderby o.GetInitialeSample() select o;
        objects = _objects.ToList();
    }

    public static void LogInNoteObject(NoteTool note, params object[] args)
    {
        NoteObj noteObj = new NoteObj();

        switch (note)
        {
            case NoteTool.TAP:
                Instance.keyData = (int)args[0];
                Instance.sampleData = (long)args[1];
                Instance.typeData = (int)args[2];

                noteObj.SetKeyID((uint)Instance.keyData);
                noteObj.SetInitialSample(Instance.sampleData);
                noteObj.SetType((NoteObj.NoteObjType)Instance.typeData);

                Instance.objects.Add(noteObj);
                break;

            case NoteTool.HOLD:
                Instance.keyData = (int)args[0];
                Instance.sampleData = (long)args[1];
                Instance.typeData = (int)args[2];
                Instance.finishSample = (long)args[3];
                break;

            case NoteTool.TRACK:
                Instance.keyData = (int)args[0];
                Instance.sampleData = (long)args[1];
                Instance.typeData = (int)args[2];

                //TODO: Start adding points
                break;

            case NoteTool.BURST:
                Instance.keyData = (int)args[0];
                Instance.sampleData = (long)args[1];
                Instance.typeData = (int)args[2];
                Instance.burstDirection = (int)args[3];
                break;

            default:
                break;
        }

        Instance.patternSets[(int)Instance.currentPatternSet].LogObject(noteObj, ((int)Instance.tick % (int)Instance.loggerSize));
        Instance.RefreshLog();
        ShowMarksInPatternSet();
    }

    public void ChangeNoteToolType(int value)
    {
        noteTool = (NoteTool)value;
    }

    public static void WriteToFile()
    {
        //Write all data collected 
        foreach (NoteObj obj in Instance.objects)
            RoftIO.OVerrideObjects(RoftCreator.filename, RoftCreator.newSongDirectoryPath + "/", obj.AsString());
    }

    public static int GetObjectCount() => Instance.objects.Count + 1;

    public static void ShowMarksInPatternSet()
    {
        //Given the PatternSet, we have to look through all blocks in that pattern set, and
        //mark the blocks accordingly
        for (int cell = 0; cell < Instance.patternSets[(int)Instance.currentPatternSet].Size(); cell++)
        {
            if (Instance.patternSets[(int)Instance.currentPatternSet] != null &&
                !Instance.patternSets[(int)Instance.currentPatternSet].IsCellEmpty(cell))
                Instance.blocks[cell].GetComponent<Button>().image.color = Instance.c_tickNoteData;
        }
    }

    public static void MarkTickAsTimeAlter()
    {
        Instance.blocks[(int)Instance.tick].GetComponent<Button>().image.color = Instance.c_tickTimeData;
    }
}
