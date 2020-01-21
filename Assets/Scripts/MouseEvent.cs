using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class MouseEvent : MonoBehaviour
{
    public static MouseEvent Instance;

    #region Public Members
    //Initial Sensitivity
    [Header("Sensitivity"), Range(1f, 20f)]
    public float initialSensitivity = 5f;

    //Refresh Rate: How long it'll take to reset to center to read direction
    [Header("Refresh Rate")]
    public float refreshRate = 0.5f;

    //Our TMP in game to tell which direction we're going
    public TextMeshProUGUI TMP_MOUSEDIRECTION;
    #endregion

    #region Private Members

    //Get vector3 for mouse position
    public Vector3 m_mousePosition;

    //The general direction of mouse cursor
    public float hdir = 2, vdir = 2;

    //Default Sensitivity
    const float defaultSensitivity = 5f;

    //Set Sensitivity
    public float sensitivity;

    //Arrow Key Counter Parts
    KeyCode[] ArrowDirectionKeys = {
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.LeftArrow,
        KeyCode.DownArrow
    };

    //Time
    float time;

    //Input Value for Mouse
    public int mouseMovementInput = 0;
    const int activeInput = 1;
    const int inactiveInput = 0;

    //Reset
    const uint reset = 0;
    public Vector3 resetPosition;
    public Camera m_camera;
    string[] horizontalString =
    {
        "<Left>", "<Right>", "<Null>"
    };
    string[] verticalString =
    {
        "<Up>", "<Down>", "<Null>"
    };
    public int horIndex = 2;
    public int verIndex = 2;

    float arrowDelaytime;
    float keyResponsiveDuration = 0.5f;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ParseArrowsToMouse();
        RunRefreshRate();
        CalculateXDifference();
        CalculateYDifference();
    }

    float GetSensitivity()
    {
        sensitivity = (defaultSensitivity + ((defaultSensitivity + 1) - initialSensitivity)) * 10;
        return sensitivity;
    }

    void RunRefreshRate()
    {
        time += Time.fixedDeltaTime;

        if (time > refreshRate)
        {
            hdir = reset;
            vdir = reset;
            time = reset;
            UpdateResetPosition();
        }
    }

    public void Refresh()
    {
        hdir = reset;
        vdir = reset;
        time = reset;
        UpdateResetPosition();
    }

    //As an alternative, you could use arrow keys opposed to the mouse
    void ParseArrowsToMouse()
    {
        foreach (KeyCode key in ArrowDirectionKeys)
        {
            if (Input.GetKey(key))
            {
                mouseMovementInput = 1;
                switch (key)
                {
                    case KeyCode.RightArrow:
                        horIndex = 1;
                        break;
                    case KeyCode.UpArrow:
                        verIndex = 0;
                        break;
                    case KeyCode.LeftArrow:
                        horIndex = 0;
                        break;
                    case KeyCode.DownArrow:
                        verIndex = 1;
                        break;
                };
            }
        }
    }

    float CalculateXDifference()
    {
        m_mousePosition = Input.mousePosition;

        hdir = resetPosition.x - m_mousePosition.x;
        return hdir;
    }

    float CalculateYDifference()
    {
        m_mousePosition = Input.mousePosition;

        vdir = resetPosition.y - m_mousePosition.y;
        return vdir;
    }

    void UpdateResetPosition()
    {
        Cursor.lockState = CursorLockMode.Locked;
        resetPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f);
    }

    public void StartArrowKeyDelay()
    {
        arrowDelaytime += Time.deltaTime;
        if (arrowDelaytime > keyResponsiveDuration)
        {
            mouseMovementInput = (int)reset;
            arrowDelaytime = reset;
        }
    }

    public int GetMouseInputValue()
    {
        return mouseMovementInput;
    }
}
