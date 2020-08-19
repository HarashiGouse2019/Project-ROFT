using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLogger : MonoBehaviour
{
    //Logger Size is simply how many blocks the object has
    public int loggerSize = 4;

    public GameObject prefab;

    readonly Color currentTick = Color.white;
    readonly Color emptyTick = Color.black;

    [SerializeField]
    float bpm = 120;
    float oldBPM = 0;

    [SerializeField]
    GameObject[] blocks;

    RectTransform rectTransform;

    const float MINUTEINSEC = 60;


    [SerializeField]
    bool startTicking = false;
    bool isTicking = false;

    [SerializeField]
    int tapsAmount = 0;
    int ticks = 0;
    List<float> loggedTime = new List<float>();
    float time;

    [SerializeField]
    float firstTap = 0;

    bool isTapping;
    bool started = false;

    const float reset = 0f;

    float ticksDelta;

    // Start is called before the first frame update
    void Start()
    {
        Init(loggerSize);
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
        ref float delta = ref ticksDelta;
        delta = MINUTEINSEC / bpm;

        

        if (startTicking && isTicking == false)
        {
            InvokeRepeating("Tick", 0, delta);
            isTicking = true;
        }

        //Detect change in BPM
        if(isTicking && bpm != oldBPM)
        {
            CancelInvoke("Tick");
            oldBPM = bpm;
            InvokeRepeating("Tick", delta / 4, delta);
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

            blocks[iter].GetComponent<Button>().image.color = iter < 1 ? currentTick : emptyTick;
        }

        StartCoroutine(TapCoroutine());
    }

    void Tick()
    {
        AudioManager.Play("Tick", 100f, true);
        ticks++;
        for(int iter = 0; iter < blocks.Length; iter++)
        {
            if(ticks % loggerSize == iter)
            {
                blocks[iter].GetComponent<Button>().image.color = currentTick;
            }
            else
            {
                blocks[iter].GetComponent<Button>().image.color = emptyTick;
            }
        }
    }

    IEnumerator TapCoroutine()
    {
        while (true)
        {
            time += Time.deltaTime * 1000;

            if (Input.GetKeyDown(KeyCode.T) && tapsAmount == 0)
            {
                isTapping = true;
                started = true;
                Tap();
            }

            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (tapsAmount == 1)
                    firstTap = time;
                CalculateAverageBPM();
                Tap();
            }

            yield return null;
        }
    }

    void Tap()
    {
        tapsAmount++;
        Debug.Log(ticks);
    }

    void CalculateAverageBPM()
    {
        bpm = tapsAmount / ((time - firstTap) / 1000 / MINUTEINSEC);
    }

}
