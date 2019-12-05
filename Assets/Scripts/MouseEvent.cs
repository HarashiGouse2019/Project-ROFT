using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class MouseEvent : MonoBehaviour
{
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
    public float hdir = 0, vdir = 0;

    //Default Sensitivity
    const float defaultSensitivity = 5f;

    //Set Sensitivity
    public float sensitivity;

    //Time
    float time;

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
    public int horIndex = 0;
    public int verIndex = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_mousePosition = Input.mousePosition;
        LockToCenter();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RunMouseDetection();
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
            LockToCenter();
            hdir = reset;
            vdir = reset;
            time = reset;
        }
    }

    public void Refresh()
    {
        LockToCenter();
        hdir = reset;
        vdir = reset;
        time = reset;
        horIndex = 2;
        verIndex = 2;
    }

    void RunMouseDetection()
    {
        Cursor.lockState = CursorLockMode.None;
        //Get the difference of position in all directions
        if (Mathf.Abs(hdir) > GetSensitivity())
        {
            switch (Mathf.Sign(hdir))
            {
                case 1: horIndex = 1; break;
                case -1: horIndex = 0; break;
            }
        }

        if (Mathf.Abs(vdir) > GetSensitivity())
        {
            switch (Mathf.Sign(vdir))
            {
                case -1: verIndex = 1; break;
                case 1: verIndex = 0; break;
            }
        }

        RunRefreshRate();
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


    void LockToCenter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        resetPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f);
    }
}
