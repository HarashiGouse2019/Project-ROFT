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
        if (dispose && ClosestObjectClass.closestObject[keyNumPosition] != null)
        {
            ClosestObjectClass.closestObject[keyNumPosition].SetActive(false);
            ClosestObjectClass.closestObject[keyNumPosition] = null;
        } 
    }
    void CloseIn()
    {
        InHitRange();

        
        transform.localScale = new Vector3(1 / GetPercentage(), 1 / GetPercentage(), 1f);

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.2f);
        if (EditorToolClass.musicSource.timeSamples > (initiatedNoteSample + accuracyVal[3]))
        {
            dispose = true;

            if (ClosestObjectClass.closestObject[keyNumPosition] == null)
                ClosestObjectClass.closestObject[keyNumPosition] = gameObject;

            BreakComboChain();

            GameManager.consecutiveMisses++;
        }

        if (accuracyString != "" && Input.GetKeyDown(Key_Layout.Instance.bindedKeys[keyNumPosition]))
        {
            dispose = true;

            if (ClosestObjectClass.closestObject[keyNumPosition] == null)
            {
                ClosestObjectClass.closestObject[keyNumPosition] = gameObject;
                IncrementComboChain();
                SendAccuracyScore();
                BuildStress(index);

                GameObject key = Key_Layout.keyObjects[keyNumPosition];
                key.GetComponentInChildren<PulseEffect>().DoPulseReaction();

                AudioManager.Instance.Play("Normal", 100, true);

                GameManager.consecutiveMisses = 0;
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
        for (int range = 0; range < accuracyVal.Length; range++)
        {
            bool beforePerfect = EditorToolClass.musicSource.timeSamples >= (initiatedNoteSample) - accuracyVal[range];
            bool afterPerfect = EditorToolClass.musicSource.timeSamples <= (initiatedNoteSample) + accuracyVal[range];

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
            sign.transform.position = ClosestObjectClass.closestObject[keyNumPosition].transform.position;
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
        BuildStress(4);
        GameManager.Instance.combo = m_break;

        GameObject sign = GetComponentInParent<ObjectPooler>().GetMember("Signs");

        if (!sign.activeInHierarchy)
        {
            sign.SetActive(true);
            sign.transform.position = gameObject.transform.position;
            sign.GetComponent<AccuracySign>().ShowSign("miss");

        }
    }

    public void BuildStress(int _index)
    {
        GameManager.sentStress = GameManager.stressAmount[_index] + (GameManager.Instance.stressBuild / 100) ;
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