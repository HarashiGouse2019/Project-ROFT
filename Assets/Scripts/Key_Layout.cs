using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using Cakewalk.IoC;
using UnityEngine.AI;
using ROFTIO = ROFTIOMANAGEMENT.RoftIO;

//This is going to be an abstract class. This will be the base class of all other layouts in the game
public class Key_Layout : MonoBehaviour
{
    [Dependency]
    public static Key_Layout Instance;

    //So I want to be able to allow the player to freely keybind key layouts (even though there's hardly any reason besides 4x4, 8x8, and 12x12)
    //Other than that, I want to go through a process of getting all homerow, toprow, and bottomrow keys.
    //KeyLayout will be given an enumerator

    public bool recordKeyInput;

    readonly NavMeshAgent test;

    #region Public Members
    public enum KeyLayoutType
    {
        //Basic literacy
        Layout_1x4,
        Layout_2x4,
        Layout_3x4,
        Layout_4x4,

        //Intermediate literacy
        Layout_3x6,
        Layout_4x6,

        //Advanced literarcy
        Layout_3x8,
        Layout_4x8,

        //Expert literacy
        Layout_3x10,
        Layout_4x10
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

    public KeyLayoutType KeyLayout;

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

    //primaryLayout is the key layout where you control
    //both ends of the in-game layout
    private readonly string[] primaryLayout = new string[10]
    {
        "asdf", // 1 x 4
        "qwerasdf", // 2 x 4
        "qwerasdfzxcv", // 3 x 4
        "1234qwerasdfzxcv", // 4 x 4
        "qwertyasdfghzxcvbn", // 3 x 6
        "123456qwertyasdfghzxcvbn", // 4 x 6
        "qweruiopasdfjkl;zxcvm,./", // 3 x 8
        "12347890qweruiopasdfjkl;zxcvm,./", // 4 x 8
        "qwertyuiopasdfghjkl;zxcvbnm,./", // 3 x 10
        "1234567890qwertyuiopasdfghjkl;zxcvbnm,./" //4 x 10
    };

    //secondaryLayout is other key bindings that also affect
    //one end of the in-game layout
    private readonly string[] secondaryLayout = new string[10]
    {
        "asl;", // 1 x 4
        "qwopasl;", // 2 x 4
        "qwopasl;zx./", // 3 x 4
        "1290qwopasl;zx./",// 4 x 4
        "qweiopasdkl;zxc,./", // 3 x 6
        "123890qweiopasdkl;zxc,./", // 4 x 6
        "qweruiopasdfjkl;zxcvm,./", // 3 x 8
        "12347890qweruiopasdfjkl;zxcvm,./", // 4 x 8
        "qwertyuiopasdfghjkl;zxcvbnm,./", //3 x 10
        "1234567890qwertyuiopasdfghjkl;zxcvbnm,./" //4 x 10
    };

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
        if (Instance == null)
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

        for (int keyID = 0; keyID < primaryLayout[(int)KeyLayout].Length; keyID++)
            InvokeKeyBind(primaryLayout[(int)KeyLayout][keyID], _rank: "primary");

        //Then I bind the secondary layout
        for (int keyID = 0; keyID < secondaryLayout[(int)KeyLayout].Length; keyID++)
        {
            GameObject keyObj = keyObjects[keyID];
            //keyObj.GetComponent<ShowLetter>().SetAssignedKeyBind(secondaryLayout[(int)KeyLayout][keyID]);
            InvokeKeyBind(secondaryLayout[(int)KeyLayout][keyID], _rank: "secondary");
        }
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
            KeyLayout = RoftCreator.Instance.GetKeyLayout();

        //float xOffset = setXOffset[(int)keyLayout];
        //float yOffset = setYOffset[(int)keyLayout];

        float xOffset = keyConfig.GetXOffset[(int)KeyLayout];
        float yOffset = keyConfig.GetYOffset[(int)KeyLayout];

        GameObject newKey;
        Vector2 keyPosition;

        //Determine how many columns and rows before setting up
        switch (KeyLayout)
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

            case KeyLayoutType.Layout_3x6:
                numRows = 3; numCols = 6;
                break;

            case KeyLayoutType.Layout_4x6:
                numRows = 4; numCols = 6;
                break;

            case KeyLayoutType.Layout_3x8:
                numRows = 3; numCols = 8;
                break;

            case KeyLayoutType.Layout_4x8:
                numRows = 4; numCols = 8;
                break;

            case KeyLayoutType.Layout_3x10:
                numRows = 3; numCols = 10;
                break;
            case KeyLayoutType.Layout_4x10:
                numRows = 4; numCols = 10;
                break;

            default:
                break;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                newKey = pooler.GetMember("keys");

                ShowLetter letter = newKey.GetComponent<ShowLetter>();

                if (!newKey.activeInHierarchy)
                {
                    newKey.SetActive(true);

                    newKey.transform.position = transform.position;
                    newKey.transform.rotation = Quaternion.identity;
                }

                newXPosition = (newKey.transform.localPosition.x + (numCols + (keyConfig.GetHorizontalSpread[(int)KeyLayout] * col)));
                newYPosition = (newKey.transform.localPosition.y - (numRows + (keyConfig.GetVerticalSpread[(int)KeyLayout] * row)));

                keyPosition = new Vector2(newXPosition, newYPosition);
                newKey.transform.localPosition = keyPosition;

                newKey.transform.localScale = new Vector3(keyConfig.GetDefaultKeyScale[(int)KeyLayout], keyConfig.GetDefaultKeyScale[(int)KeyLayout]);

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