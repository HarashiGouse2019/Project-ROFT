using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class MouseEvent : MonoBehaviour
{
    public static MouseEvent Instance;

    #region Public Members
    //Refresh Rate: How long it'll take to reset to center to read direction
    [Header("Refresh Rate")]
    public float refreshRate = 0.5f;

    //Our TMP in game to tell which direction we're going
    public TextMeshProUGUI TMP_MOUSEDIRECTION;
    #endregion

    #region Private Members

    //The general direction of mouse cursor
    public float hdir = 2, vdir = 2;

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

    //Reset
    const uint reset = 0;
    public Vector3 resetPosition;

    public int horIndex = 2;
    public int verIndex = 2;

    float arrowDelaytime;
    float keyResponsiveDuration = 0.5f;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ArrowInputs();
        RunRefreshRate();
    }

    void RunRefreshRate()
    {
        time += Time.fixedDeltaTime;

        if (time > refreshRate)
        {
            hdir = reset;
            vdir = reset;
            time = reset;
        }
    }

    public void Refresh()
    {
        hdir = reset;
        vdir = reset;
        time = reset;
    }

    //As an alternative, you could use arrow keys opposed to the mouse
    void ArrowInputs()
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
