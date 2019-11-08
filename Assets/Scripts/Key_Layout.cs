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
        Layout_HomeRow,
        Layout_3Row
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

    public KeyLayoutType keyLayout;

    public LayoutMethod layoutMethod;

    public float[] setXOffset;
    public float[] setYOffset;

    public bool autoBindKeys = true;

    [Header("Data Ui")]
    public TextMeshProUGUI dataText;

    //After iterating through strings, we'll return a Input corresponding avaliable keys.
    readonly public List<KeyCode> bindedKeys = new List<KeyCode>();
    public static List<GameObject> keyObjects = new List<GameObject>();
    public List<GameObject> tempShowKeyObjs = new List<GameObject>();

    [Header("Creating Layout")]
    public GameObject key;
    public ObjectPooler pooler;

    public float spawnPositionX = 0;
    public float spawnPositionY = 0;

    #endregion

    #region Private Members
    readonly private string[] defaultLayout = new string[5]
    {
        "asl;",
        "qwopasl;",
        "qwopasl;zx./",
        "asdfghjkl;",
        "qwertyuiopasdfghjkl;zxcvbnm,./"
    };
    readonly private float[] defaultKeyScale = new float[5]
    {
        2.25f,
        1.5f,
        1.4f,
        1.4f,
        1f
    };
    readonly private float[] keySpread = new float[5] {
        3.5f,
        2.5f,
        2f,
        2f,
        1.5f
    };

    private float newXPosition = 0f;
    private float newYPosition = 0f;
    private uint numCols = 0;
    private uint numRows = 0;
    public Camera m_camera;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (
            GameManager.Instance.gameMode == GameManager.GameMode.TBR_ALL ||
            GameManager.Instance.gameMode == GameManager.GameMode.TBR_HOMEROW)
            ChangeLayoutMethod(1);
        else
            ChangeLayoutMethod(0);
    }

    private void Update()
    {
        tempShowKeyObjs = keyObjects;
    }

    void InitiateAutoKeyBind()
    {
        //Find all objects in parent
        for (int keyNum = 0; keyNum < defaultLayout[(int)keyLayout].Length; keyNum++)
            InvokeKeyBind(defaultLayout[(int)keyLayout][keyNum]);
    }

    //This will simply take any character, and keybind it.
    public KeyCode InvokeKeyBind(char m_char)
    {

        KeyCode key;
        key = (KeyCode)m_char;
        bindedKeys.Add(key);
        return key;
    }

    //This takes any ASCII integer that exists on the keyboard
    //if the player so desires to manually keybind
    public KeyCode InvokeKeyBind(int m_int)
    {
        KeyCode key;
        key = (KeyCode)m_int;
        bindedKeys.Add(key);
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

        float xOffset = setXOffset[(int)keyLayout];
        float yOffset = setYOffset[(int)keyLayout];

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
            case KeyLayoutType.Layout_HomeRow:
                numRows = 1; numCols = 10;
                break;
            case KeyLayoutType.Layout_3Row:
                numRows = 3; numCols = 10;
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

                newXPosition = (newKey.transform.localPosition.x + (numCols + (keySpread[(int)keyLayout] * col)));
                newYPosition = (newKey.transform.localPosition.y - (numRows + (keySpread[(int)keyLayout] * row)));

                keyPosition = new Vector2(newXPosition, newYPosition);
                newKey.transform.localPosition = keyPosition;
                newKey.transform.localScale = new Vector3(defaultKeyScale[(int)keyLayout], defaultKeyScale[(int)keyLayout]);

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

    //This will be called based on frames. This will use the
    //TypeByRegion class to do it's magic.
    //There's 2 Stages or Processes
    public KeyCode RandomizeAndProcess()
    {
        #region Cell Number Process
        TypeByRegion recipient = TypeByRegion.Instance;

        float randX = Random.Range(TypeByRegion.left, TypeByRegion.screenWidth - TypeByRegion.right);
        float randY = Random.Range(TypeByRegion.bottom, TypeByRegion.screenHeight - TypeByRegion.top);

        //Give newXPosition and newYPosition for the key
        spawnPositionX = randX;
        spawnPositionY = randY;

        //Make a Vector2 with our random coordinates
        Vector2 positionInScreen = new Vector2(randX, randY);

        //We get our Regional Position
        Vector2 regionalPosition = recipient.CheckRegionalPositionFrom(positionInScreen);

        //Now we get the Cell Number
        int cellNum = recipient.RegionalPositionToCellNumber(regionalPosition);

        //With our cellNum, we can now tell our recipient to give use the keyClusters that
        //We need
        string usedKeyCluster = recipient.GetKeyClusterFromCellNum(cellNum);
        #endregion

        #region Key Binding Process
        //Now we randomize a number between
        //0 and the length of the key cluster to get
        //the desired key to bind
        int randNum = Random.Range(0, usedKeyCluster.Length);
        char randChar = usedKeyCluster[randNum];

        //And now, parse to KeyCode
        return InvokeKeyBind(randChar);
        #endregion
    }

    //This function will directly change the layout mode.
    //0 is Abstract, 1 is Scatter
    int ChangeLayoutMethod(int _mode)
    {
        layoutMethod = (LayoutMethod)_mode;
        return _mode;
    }
}