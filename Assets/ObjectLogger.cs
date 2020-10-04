using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLogger : MonoBehaviour
{
    public ObjectLogger Instance;
    public TextMeshProUGUI TMP_Tick;
    public enum LoggerSize {
        WALTZ = 3,
        WHOLE = 4,
        SIXTH = 6,
        EIGHTH = 8,
        SIXTEEN = 16,
        THRITY_SECNOND = 32,
        SIXTY_FORTH = 64,
        ONE_HUNTRED_TWENTY_EIGHTH = 128
    }

    public LoggerSize loggerSize = LoggerSize.WHOLE;

    [Range(1f, 4f)]
    public float rate = 1f;

    public float offsetInSeconds = 0;

    public GameObject prefab;

    readonly Color currentTick = Color.white;
    readonly Color emptyTick = Color.black;
    readonly Color emptyTickGrey = Color.grey;

    [SerializeField]
    public float bpm = 120;
    float oldBPM = 0;

    [SerializeField]
    GameObject[] blocks;

    RectTransform rectTransform;

    const float MINUTEINSEC = 60;


    [SerializeField]
    bool startTicking = false;
    bool isTicking = false;

    List<float> loggedTime = new List<float>();
    float time;

    float tick = -1;
    float inGameTime = 0;

    const float reset = 0f;

    private void OnEnable()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Init((int)loggerSize);
        oldBPM = bpm;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            startTicking = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inGameTime = RoftPlayer.musicSource.time - offsetInSeconds;

        var currentTick = Mathf.Floor(inGameTime / (60f / (bpm * rate * ((float)loggerSize / (float)LoggerSize.WHOLE))));

        if (tick != currentTick && inGameTime > 0f)
        {
            tick = currentTick;
            TMP_Tick.text = ((tick % (int)loggerSize) + 1).ToString();
            if (Mathf.Floor(inGameTime % (60f / bpm)) == 0)
                Tick();
        }
    }


    void Init(int size)
    {
        blocks = new GameObject[size];

        for (int iter = 0; iter < size; iter++)
        {
            blocks[iter] = Instantiate(prefab);
            blocks[iter].transform.parent = transform.parent;
            blocks[iter].transform.position = transform.position + new Vector3(1f * iter, 0f, 0f);
            blocks[iter].transform.rotation = Quaternion.identity;
            blocks[iter].transform.localScale = Vector3.one;

            blocks[iter].GetComponent<Button>().image.color = iter < 1 ? currentTick : iter % ((int)loggerSize/4) != 0 ? emptyTickGrey : emptyTick;
        }

    }

    void Tick()
    {
        for(int iter = 0; iter < blocks.Length; iter++)
        {
            if(tick % (int)loggerSize == iter)
            {
                blocks[iter].GetComponent<Button>().image.color = currentTick;
            }
            else
            {
                if (iter % ((int)loggerSize/4) != 0)
                    blocks[iter].GetComponent<Button>().image.color = emptyTickGrey;
                else
                    blocks[iter].GetComponent<Button>().image.color = emptyTick;
            }
        }
    }
}
