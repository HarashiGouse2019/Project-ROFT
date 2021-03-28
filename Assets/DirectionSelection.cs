using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

public class DirectionSelection : MonoBehaviour
{
    [SerializeField, Header("Arrow Ui Image Targets")]
    private Image[] arrowUI = new Image[4];

    //Valid Inputs
    private Dictionary<KeyCode, int> directionInputDictionary;

    [SerializeField, Header("Color Selection")]
    private Color normalColor, pressedColor;

    [SerializeField]
    private int setValue = 0; //right direction is default

    private void OnEnable()
    {
        //Start coroutine on enable
        StartCoroutine(SelectionCycle());
    }

    private void Init()
    {
        IncrementerInt32 inc = new IncrementerInt32(0);

        //Initialize Dictionaries
        directionInputDictionary.Add(KeyCode.RightArrow, inc.Next);
        directionInputDictionary.Add(KeyCode.UpArrow, inc.Next);
        directionInputDictionary.Add(KeyCode.LeftArrow, inc.Next);
        directionInputDictionary.Add(KeyCode.DownArrow, inc.Next);

        inc.Reset();
    }

    void CheckControls()
    {
        foreach (KeyValuePair<KeyCode, int> input in directionInputDictionary) {
            if (Input.GetKeyDown(input.Key)) Highlight(input.Value); else UnHighlight();
         }
    }

    void Highlight(int index)
    {
        setValue = index;
        arrowUI[setValue].color = pressedColor;
    }
    
    void UnHighlight()
    {
        for (int i = 0; i < arrowUI.Length; i++)
        {
            if (i == setValue) continue;
            arrowUI[i].color = normalColor;
        }
    }

    IEnumerator SelectionCycle()
    {
        //Initialize
        Init();

        while (true)
        {
            CheckControls();
            yield return null;
        }
    }
}
