using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

//This is going to be an abstract class. This will be the base class of all other layouts in the game
public class Key_Layout : MonoBehaviour
{
    public static Key_Layout Instance;
    //So I want to be able to allow the player to freely keybind key layouts (even though there's hardly any reason besides 4x4, 8x8, and 12x12)
    //Other than that, I want to go through a process of getting all homerow, toprow, and bottomrow keys.
    //KeyLayout will be given an enumerator

    public bool recordKeyInput;

    #region Public Members
    public enum KeyLayoutType
    {
        Layout_1x4,
        Layout_2x4,
        Layout_3x4,
        Layout_4x4
    }

    //This enum will change depending on the game mode in 
    //the game manager
    //Automatically, the KeyLayoutType mode will be either
    //Layout_HomeRow or Layout_3Row
    public enum LayoutMethod
    {
        Abstract,
        Region_Scatter
    }

    public KeyLayoutType keyLayout { get; set; }

    public LayoutMethod layoutMethod;

    public bool autoBindKeys = true;

    [Header("Data Ui")]
    public TextMeshProUGUI dataText;

    //After iterating through strings, we'll return a Input corresponding avaliable keys.
    public List<KeyCode> primaryBindedKeys = new List<KeyCode>();
    public List<KeyCode> secondaryBindedKeys = new List<KeyCode>();

    public static List<GameObject> keyObjects = new List<GameObject>();
    public List<GameObject> tempShowKeyObjs = new List<GameObject>();

    [Header("Creating Layout")]
    public GameObject key;
    public ObjectPooler pooler;

    #endregion

    #region Private Members
    #region Subject to Removal/Modification
    //primaryLayout is the key layout where you control
    //both ends of the in-game layout
    private readonly string[] primaryLayout = new string[4]
    {
        "asdf",
        "qwerasdf",
        "qwerasdfzxcv",
        "asdfghjkl;",
    };

    //secondaryLayout is other key bindings that also affect
    //one end of the in-game layout
    private readonly string[] secondaryLayout = new string[4]
    {
        "asl;",
        "qwopasl;",
        "qwopasl;zx./",
        "1290qwopasl;zx./"
    };

    #endregion

    private KeyConfig keyConfig;

    //newX and newY are for Abstract Layout
    private float newXPosition = 0f;
    private float newYPosition = 0f;
    private uint numCols = 0;
    private uint numRows = 0;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        keyConfig = new KeyConfig();
        keyConfig = JsonUtility.FromJson<KeyConfig>(keyConfig.GetJSONString());
    }

    private void Update()
    {
        tempShowKeyObjs = keyObjects;
    }

    void InitiateAutoKeyBind()
    {
        //I want to first bind the primary layout
        for (int keyID = 0; keyID < primaryLayout[(int)keyLayout].Length; keyID++)
            InvokeKeyBind(primaryLayout[(int)keyLayout][keyID], _rank: "primary");

        //Then I bind the secondary layout
        for (int keyID = 0; keyID < secondaryLayout[(int)keyLayout].Length; keyID++)
            InvokeKeyBind(secondaryLayout[(int)keyLayout][keyID], _rank: "secondary");
    }

    //This will simply take any character, and keybind it.
    public KeyCode InvokeKeyBind(char m_char, bool _addToList = true, string _rank = "primary")
    {

        KeyCode key;
        key = (KeyCode)m_char;

        if (_addToList)
        {
            switch (_rank.ToLower())
            {
                case "primary":
                    primaryBindedKeys.Add(key);
                    break;
                case "secondary":
                    secondaryBindedKeys.Add(key);
                    break;
            }
        }

        return key;
    }

    //This takes any ASCII integer that exists on the keyboard
    //if the player so desires to manually keybind
    public KeyCode InvokeKeyBind(int m_int, bool _addToList = true, string _rank = "primary")
    {
        KeyCode key;
        key = (KeyCode)m_int;

        if (_addToList)
        {
            switch (_rank.ToLower())
            {
                case "primary":
                    primaryBindedKeys.Add(key);
                    break;
                case "secondary":
                    secondaryBindedKeys.Add(key);
                    break;
            }
        }
        return key;

    }

    public void SetUpLayout()
    {
        //So I figured out the problem to the build problem, and it has to do with the referencing of keys.
        //I don't want to manually do it for all of them, so instead, I'll create a function that'll do it for me.
        //Probably 2 or 3 times the work, but I don't have to click and drag stuff, and all that good stuff

        //We need a variable that spreads the individual keys out
        //Almost like Procedural Generating, but uniformed... I think...

        //I don't think I want it adjustable by the designer... but for the implementation of it, we will.

        //We do a double for loop! Columns and Rows (At least for the 8x8, 12x12, and 3Row

        if (RoftPlayer.Instance.record)
            keyLayout = RoftCreator.Instance.GetKeyLayout();

        //float xOffset = setXOffset[(int)keyLayout];
        //float yOffset = setYOffset[(int)keyLayout];

        float xOffset = keyConfig.GetXOffset[(int)keyLayout];
        float yOffset = keyConfig.GetYOffset[(int)keyLayout];

        GameObject newKey;
        Vector2 keyPosition;

        //Determine how many columns and rows before setting up
        switch (keyLayout)
        {
            case KeyLayoutType.Layout_1x4:
                numRows = 1; numCols = 4;
                break;
            case KeyLayoutType.Layout_2x4:
                numRows = 2; numCols = 4;
                break;
            case KeyLayoutType.Layout_3x4:
                numRows = 3; numCols = 4;
                break;
            case KeyLayoutType.Layout_4x4:
                numRows = 4; numCols = 4;
                break;
            default:
                break;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                newKey = pooler.GetMember("keys");

                if (!newKey.activeInHierarchy)
                {
                    newKey.SetActive(true);

                    newKey.transform.position = transform.position;
                    newKey.transform.rotation = Quaternion.identity;

                    newKey.GetComponent<SpawnedFrom>().origin = gameObject;
                }

                newXPosition = (newKey.transform.localPosition.x + (numCols + (keyConfig.GetHorizontalSpread[(int)keyLayout] * col)));
                newYPosition = (newKey.transform.localPosition.y - (numRows + (keyConfig.GetVerticalSpread[(int)keyLayout] * row)));

                keyPosition = new Vector2(newXPosition, newYPosition);
                newKey.transform.localPosition = keyPosition;

                newKey.transform.localScale = new Vector3(keyConfig.GetDefaultKeyScale[(int)keyLayout], keyConfig.GetDefaultKeyScale[(int)keyLayout]);

                keyObjects.Add(newKey);

            }
        }

        //After setting up the keys,  bring them to center 
        //And then autobind keys
        for (int keyNum = 0; keyNum < keyObjects.Count; keyNum++)
        {
            Vector3 shiftPosition = keyObjects[keyNum].transform.localPosition;
            Vector2 offset = new Vector2(shiftPosition.x + xOffset, shiftPosition.y + yOffset);
            keyObjects[keyNum].transform.localPosition = offset;

            //Check if these notes are interactable
            if (recordKeyInput)
            {
                InteractableKey newInteractable = keyObjects[keyNum].AddComponent<InteractableKey>();
                newInteractable.SetKeyNum(keyNum);
                keyObjects[keyNum].GetComponent<CircleCollider2D>().enabled = true;
            }

        }
        InitiateAutoKeyBind();
    }
}